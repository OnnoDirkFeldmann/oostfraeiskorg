using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using DotVVM.Framework.Controls;
using System.Linq;
using System.Data;
using System;

namespace oostfraeiskorg;

public class Searcher
{
    /// <summary>
    /// Translation dictionary for database column names (German to English)
    /// </summary>
    private static readonly Dictionary<string, string> ColumnTranslationsEnglish = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Ostfriesisch", "East Frisian" },
        { "Deutsch", "German" },
        { "Englisch", "English" },
        { "Artikel", "Article" },
        { "Wortart", "Part of speech" },
        { "Plural", "Plural" },
        { "Nebenformen", "Dialectal forms" },
        { "Etymologie", "Etymology" },
        { "Standardform", "Standard form" },
        { "Zuordnung", "Category" },
        { "Nummer", "Number" },
        { "Genus", "Gender" },
        { "Bemerkung", "Remark" },
        { "Beispiel", "Example" },
        { "Herkunft", "Origin" },
        { "Konjugation", "Conjugation"},
        { "Kommentar", "Comment"},
    };

    public static IQueryable<DictionaryEntry> SearchAndFill(string searchString, string searchDirection, string fullTextSearch)
    {
        var dictionaryEntries = new List<DictionaryEntry>();

        string notFoundMessage = "";
        string needInputMessage = "";
        var language = Languages.German;
        string dialectalString = "";
        searchString = searchString.Trim();
        var displaySearchString = searchString;

        switch (searchDirection)
        {
            case "de>frs":
            case "frs>de":
                notFoundMessage = $"Es sind keine Daten für die Suche '{displaySearchString}' gefunden worden";
                needInputMessage = "Sie müssen mindestens ein Wort eingeben";
                language = Languages.German;
                dialectalString = "dialektal";
                break;
            case "en>frs":
            case "frs>en":
                notFoundMessage = $"No data found for '{displaySearchString}'";
                needInputMessage = "you have to enter at least one word";
                language = Languages.English;
                dialectalString = "dialectal";
                break;
        }

        if (displaySearchString.Equals(string.Empty))
        {
            dictionaryEntries.Add(new DictionaryEntry("", "Jii mautent minst äin woord ingeeven", "", "", needInputMessage, 0));
            return dictionaryEntries.AsQueryable();
        }

        var dataBaseConnection = DataBaseConnection.GetConnection(oostfraeiskorg.Server.MapPath(""));
        var sqlcmd = new SqliteCommand
        {
            Connection = dataBaseConnection
        };

        switch (searchDirection.ToLower())
        {
            case "de>frs":
                SearchStrings.DeFrs(fullTextSearch, ref searchString, ref sqlcmd);
                break;
            case "frs>de":
                SearchStrings.FrsDe(fullTextSearch, ref searchString, ref sqlcmd);
                break;
            case "en>frs":
                SearchStrings.EnFrs(fullTextSearch, ref searchString, ref sqlcmd);
                break;
            case "frs>en":
                SearchStrings.FrsEn(fullTextSearch, ref searchString, ref sqlcmd);
                break;
        }

        var searchstrParamUpper = new SqliteParameter()
        {
            ParameterName = "@searchstrupper",
            Value = searchString.ToUpper()
        };
        var searchstrParamLower = new SqliteParameter()
        {
            ParameterName = "@searchstrlower",
            Value = searchString.ToLower()
        };
        var searchstrParam = new SqliteParameter()
        {
            ParameterName = "@searchstr",
            Value = searchString
        };

        sqlcmd.Parameters.Add(searchstrParamUpper);
        sqlcmd.Parameters.Add(searchstrParamLower);
        sqlcmd.Parameters.Add(searchstrParam);
        sqlcmd.Prepare();

        var reader = sqlcmd.ExecuteReader();
        var dictionaryRows = new List<DictionaryRow>();

        Console.Write("start reading dictionary");

        while (reader.Read())
        {
            var values = new DictionaryRow();
            values.ID = reader.GetInt64("ID");
            values.Ostfriesisch = reader.GetValue("Ostfriesisch").ToString();
            values.Deutsch = reader.GetValue("Deutsch").ToString();
            values.Englisch = reader.GetValue("Englisch").ToString();
            values.Artikel = reader.GetValue("Artikel").ToString();
            values.Nebenformen = reader.GetValue("Nebenformen").ToString();
            values.Standardform = reader.GetValue("Standardform").ToString();
            values.Wortart = GetColumnValueSafe(reader, "Wortart");
            values.Zuordnung = GetColumnValueSafe(reader, "Zuordnung");
            values.Nummer = GetColumnValueSafe(reader, "Nummer");
            dictionaryRows.Add(values);
        }

        reader.Close();

        dataBaseConnection.Close();

        Console.Write("finished reading dictionary");

        // Build entries
        var allEntries = new List<DictionaryEntry>();

        foreach (var dictionaryRow in dictionaryRows)
        {
            var eastFrisianString = dictionaryRow.Ostfriesisch;
            eastFrisianString += !dictionaryRow.Artikel.Equals("-") ? $" ({dictionaryRow.Artikel})" : "";
            var eastFrisianSecondaryForm = !dictionaryRow.Nebenformen.Equals("-") ? $"[{dialectalString}: {dictionaryRow.Nebenformen}]" : "";
            var eastFrisianStandardForm = !dictionaryRow.Standardform.Equals("-") ? $"[{dictionaryRow.Standardform}]" : "";

            string translationText = language == Languages.German ? dictionaryRow.Deutsch : dictionaryRow.Englisch;
            bool isPhrase = dictionaryRow.Wortart.Equals("Phrase", StringComparison.OrdinalIgnoreCase);
            string phraseParent = dictionaryRow.Zuordnung.Trim();
            if (phraseParent.Equals("-")) phraseParent = "";

            var dictionaryEntry = new DictionaryEntry(
                dictionaryRow.Ostfriesisch,
                eastFrisianString,
                eastFrisianSecondaryForm,
                eastFrisianStandardForm,
                translationText,
                dictionaryRow.ID,
                dictionaryRow.Nummer,
                isPhrase,
                phraseParent
            );

            if (!dictionaryRow.ID.Equals(0))
            {
                string mp3 = $"{oostfraeiskorg.Server.MapPath("")}/wwwroot/rec/{dictionaryRow.Ostfriesisch}.mp3";
                if (File.Exists(mp3))
                {
                    dictionaryEntry.SoundFile = true;
                    dictionaryEntry.MP3 = dictionaryRow.Ostfriesisch;
                }
            }

            allEntries.Add(dictionaryEntry);
        }

        // Separate words and phrases
        var words = allEntries.Where(e => !e.IsPhrase).ToList();
        var phrases = allEntries.Where(e => e.IsPhrase).ToList();

        // Sort words by relevance: prioritize entries where search term appears first in translation
        words = SortByRelevance(words, displaySearchString, searchDirection);

        // Build a lookup of words by their East Frisian name and entrynumber
        var wordLookup = new Dictionary<string, DictionaryEntry>(StringComparer.Ordinal);
        foreach (var word in words)
        {
            // Extract base word (before any parenthesis)
            var baseWord = word.Number.Equals("-") ? word.Frisian : $"{word.Frisian}={word.Number}";
            if (!wordLookup.ContainsKey(baseWord))
            {
                wordLookup[baseWord] = word;
            }
        }

        // Assign phrases to their parent words
        var orphanPhrases = new List<DictionaryEntry>();
        foreach (var phrase in phrases)
        {
            if (!string.IsNullOrEmpty(phrase.PhraseParent) && wordLookup.TryGetValue(phrase.PhraseParent, out var parentWord))
            {
                parentWord.Phrases.Add(phrase);
            }
            else
            {
                // Orphan phrase (no matching parent in results)
                orphanPhrases.Add(phrase);
            }
        }

        // Build final list: words (with their phrases attached), then orphan phrases at the bottom
        dictionaryEntries.AddRange(words);
        dictionaryEntries.AddRange(orphanPhrases);

        if (allEntries.Count == 0)
        {
            var dictionaryEntry = new DictionaryEntry("", "D'r bünt kiin dóóten föör d' söyek '" + displaySearchString + "' funnen worden", "", "", notFoundMessage, 0);
            dictionaryEntries.Add(dictionaryEntry);
        }

        return dictionaryEntries.AsQueryable();
    }

    /// <summary>
    /// Sorts dictionary entries by relevance based on where the search term appears.
    /// Priority order:
    /// 1. Search term is an exact standalone match as the first item (e.g., "gehen" or "gehen; ..." or "gehen, ...")
    /// 2. Search term appears as a standalone item elsewhere in the list
    /// 3. Search term appears at the start of a phrase (e.g., "gehen an einer Stütze")
    /// 4. Search term is contained within other text
    /// Alphabetical by Frisian word within each group
    /// </summary>
    private static List<DictionaryEntry> SortByRelevance(List<DictionaryEntry> entries, string searchTerm, string searchDirection)
    {
        var searchLower = searchTerm.ToLower();
        bool searchingFrisian = searchDirection.StartsWith("frs>", StringComparison.OrdinalIgnoreCase);

        return entries
            .OrderBy(entry =>
            {
                // Get the text to check based on search direction
                string textToCheck = searchingFrisian ? entry.Frisian : entry.Translation;
                if (string.IsNullOrEmpty(textToCheck))
                    return 5; // No match, lowest priority

                var textLower = textToCheck.ToLower();

                // Split into individual items by semicolon
                var items = textLower.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                // Check if the first item is an exact match or starts with search term followed by comma
                var firstItem = items.FirstOrDefault() ?? "";
                if (IsExactOrCommaMatch(firstItem, searchLower))
                    return 0; // Highest priority: exact match as first item

                // Check if any other item is an exact match
                for (int i = 1; i < items.Count; i++)
                {
                    if (IsExactOrCommaMatch(items[i], searchLower))
                        return 1; // High priority: exact match in list but not first
                }

                // Check if search term is the first item but followed by more words (phrase starting with search term)
                if (firstItem.StartsWith(searchLower + " "))
                    return 2; // Medium priority: first item starts with search term as a phrase

                // Check if any item starts with the search term (phrase match)
                foreach (var item in items)
                {
                    if (item.StartsWith(searchLower + " "))
                        return 3; // Lower priority: some item starts with search term
                }

                // Search term is contained somewhere
                if (textLower.Contains(searchLower))
                    return 4; // Lowest match priority

                return 5; // No direct match
            })
            .ThenBy(entry => entry.Frisian, StringComparer.OrdinalIgnoreCase) // Alphabetical within same relevance
            .ToList();
    }

    /// <summary>
    /// Checks if the item is an exact match for the search term, or if it's the search term 
    /// followed by a comma (e.g., "gehen, laufen" where we're searching for "gehen").
    /// </summary>
    private static bool IsExactOrCommaMatch(string item, string searchTerm)
    {
        // Exact match
        if (item.Equals(searchTerm))
            return true;

        // Match with comma after (e.g., "gehen, laufen" matches "gehen")
        if (item.StartsWith(searchTerm + ","))
            return true;

        // Also handle case where there might be spaces around comma
        if (item.StartsWith(searchTerm + " ,"))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the search term appears as a standalone word or at the start of a list item.
    /// </summary>
    private static bool IsStandaloneMatch(string text, string searchTerm)
    {
        // Split by common separators (semicolon, comma)
        var parts = text.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            // Check if this part equals the search term exactly
            if (trimmed.Equals(searchTerm))
                return true;
        }
        
        return false;
    }

    public static string GetPopUpBody(long wordId, Languages language = Languages.German)
    {
        string body = "";

        var dataBaseConnection = DataBaseConnection.GetConnection(oostfraeiskorg.Server.MapPath(""));
        var sqlCommand = new SqliteCommand();
        sqlCommand.Connection = dataBaseConnection;
        sqlCommand.CommandText = "SELECT * FROM WB Where ID ='" + wordId + "'";
        var reader = sqlCommand.ExecuteReader();

        var titles = new List<string>();
        var data = new List<string>();
        while (reader.Read())
        {
            try
            {
                int i = 1;
                while (true)
                {
                    if (i != 2)
                    {
                        titles.Add(reader.GetName(i));
                        data.Add(reader.GetValue(i).ToString());
                    }
                    else
                    {
                        titles.Add(reader.GetName(i));
                        data.Add(reader.GetValue(i).ToString());
                    }
                    i++;
                }
            }
            catch
            {
            }
        }
        reader.Close();
        dataBaseConnection.Close();

        var notEmptyTitles = new List<string>();
        var notEmptyData = new List<string>();
        for (int j = 0; j < data.Count; j++)
        {
            if (!data[j].Equals(string.Empty) && !data[j].Equals("-"))
            {
                notEmptyTitles.Add(titles[j]);
                notEmptyData.Add(data[j]);
            }
        }

        for (int j = 0; j < notEmptyData.Count; j++)
        {
            string displayTitle = language == Languages.English
                ? TranslateColumnName(notEmptyTitles[j])
                : notEmptyTitles[j];
            body += "<tr valign=\"top\"><th valign=\"top\"><p>" + displayTitle + ":</p></th><td valign=\"top\"><p>" + notEmptyData[j] + "</p></td ></tr>";
        }

        body = "<table class=\"table\">" + body + "</table>";
        body = body.Replace("'", "&#39;");
        body = body.Replace("\"", "&#34;");
        body = body.Replace("\r", "");
        body = body.Replace("\n", "");

        return body;
    }

    /// <summary>
    /// Translates a German column name to English. Returns original if no translation found.
    /// </summary>
    private static string TranslateColumnName(string germanName)
    {
        return ColumnTranslationsEnglish.TryGetValue(germanName, out var englishName)
            ? englishName
            : germanName;
    }

    private static string GetColumnValueSafe(SqliteDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.GetValue(ordinal)?.ToString() ?? "-";
        }
        catch (ArgumentOutOfRangeException)
        {
            return "-";
        }
    }
}