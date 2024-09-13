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
            dictionaryRows.Add(values);
        }

        reader.Close();
        dataBaseConnection.Close();

        Console.Write("finished reading dictionary");

        var idList = new List<long>();
        var eastFrisianStrings = new List<string>();
        var eastFrisianSecondaryForms = new List<string>();
        var eastFrisianStandardForms = new List<string>();
        var eastFrisianWords = new List<string>();
        var translation = new List<string>();

        foreach (var dictionaryRow in dictionaryRows)
        {
            idList.Add(dictionaryRow.ID);
            var eastFrisianString = dictionaryRow.Ostfriesisch;
            eastFrisianString += !dictionaryRow.Artikel.Equals("-") ? $" ({dictionaryRow.Artikel})" : "";
            var eastFrisianSecondaryForm = !dictionaryRow.Nebenformen.Equals("-") ? $"[{dialectalString}: {dictionaryRow.Nebenformen}]" : "";
            var eastFrisianStandardForm = !dictionaryRow.Standardform.Equals("-") ? $"[{dictionaryRow.Standardform}]" : "";

            eastFrisianStrings.Add(eastFrisianString);
            eastFrisianSecondaryForms.Add(eastFrisianSecondaryForm);
            eastFrisianStandardForms.Add(eastFrisianStandardForm);
            switch (language)
            {
                case Languages.German:
                    translation.Add(dictionaryRow.Deutsch);
                    break;
                case Languages.English:
                    translation.Add(dictionaryRow.Englisch);
                    break;
            }
            eastFrisianWords.Add(dictionaryRow.Ostfriesisch);
        }

        for (int i = 0; i < idList.Count; i++)
        {
            var dictionaryEntry = new DictionaryEntry(eastFrisianStrings[i], eastFrisianSecondaryForms[i], eastFrisianStandardForms[i], translation[i], idList[i]);

            if (!idList[i].Equals("0"))
            {
                string mp3 = $"{oostfraeiskorg.Server.MapPath("")}/wwwroot/rec/{eastFrisianWords[i]}.mp3";
                if (File.Exists(mp3))
                {
                    dictionaryEntry.SoundFile = true;
                    dictionaryEntry.MP3 = eastFrisianWords[i];
                }
            }

            dictionaryEntries.Add(dictionaryEntry);
        }

        if (eastFrisianStrings.Count == 0)
        {
            var dictionaryEntry = new DictionaryEntry("D'r bünt kiin dóóten föör d' söyek '" + displaySearchString + "' funnen worden", "", "", notFoundMessage, 0);
            dictionaryEntries.Add(dictionaryEntry);
        }

        return dictionaryEntries.AsQueryable();
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
}