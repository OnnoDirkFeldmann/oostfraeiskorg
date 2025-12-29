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
            dictionaryEntries.Add(new DictionaryEntry("Jii mautent minst äin woord ingeeven", "", "", needInputMessage, 0));
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
            dictionaryRows.Add(values);
        }

        reader.Close();

        // Fetch Zuordnung from WB table for entries where it's missing
        var idsNeedingZuordnung = dictionaryRows
            .Where(r => r.Zuordnung == "-" && r.ID != 0)
            .Select(r => r.ID)
            .ToList();

        if (idsNeedingZuordnung.Count > 0)
        {
            var zuordnungLookup = GetZuordnungForIds(dataBaseConnection, idsNeedingZuordnung);
            foreach (var row in dictionaryRows)
            {
                if (row.Zuordnung == "-" && zuordnungLookup.TryGetValue(row.ID, out var zuordnung))
                {
                    row.Zuordnung = zuordnung;
                }
            }
        }

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
                eastFrisianString,
                eastFrisianSecondaryForm,
                eastFrisianStandardForm,
                translationText,
                dictionaryRow.ID,
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

        // Build a lookup of words by their East Frisian name (without article)
        var wordLookup = new Dictionary<string, DictionaryEntry>(StringComparer.OrdinalIgnoreCase);
        foreach (var word in words)
        {
            // Extract base word (before any parenthesis)
            var baseWord = word.Frisian.Split('(')[0].Trim();
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
            var dictionaryEntry = new DictionaryEntry("D'r bünt kiin dóóten föör d' söyek '" + displaySearchString + "' funnen worden", "", "", notFoundMessage, 0);
            dictionaryEntries.Add(dictionaryEntry);
        }

        return dictionaryEntries.AsQueryable();
    }

    private static Dictionary<long, string> GetZuordnungForIds(SqliteConnection connection, List<long> ids)
    {
        var result = new Dictionary<long, string>();
        if (ids.Count == 0) return result;

        // Build a query to fetch Zuordnung for all IDs
        var idList = string.Join(",", ids);
        var sqlCommand = new SqliteCommand
        {
            Connection = connection,
            CommandText = $"SELECT ID, Zuordnung FROM WB WHERE ID IN ({idList})"
        };

        var reader = sqlCommand.ExecuteReader();
        while (reader.Read())
        {
            var id = reader.GetInt64(0);
            var zuordnung = reader.GetValue(1)?.ToString() ?? "-";
            result[id] = zuordnung;
        }
        reader.Close();

        return result;
    }

    public static string GetPopUpBody(long wordId)
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
            body += "<tr valign=\"top\"><th valign=\"top\"><p>" + notEmptyTitles[j] + ":</p></th><td valign=\"top\"><p>" + notEmptyData[j] + "</p></td ></tr>";
        }

        body = "<table class=\"table\">" + body + "</table>";
        body = body.Replace("'", "&#39;");
        body = body.Replace("\"", "&#34;");
        body = body.Replace("\r", "");
        body = body.Replace("\n", "");

        return body;
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