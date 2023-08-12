using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.IO;
using DotVVM.Framework.Controls;
using static oostfraeiskorg.ViewModels.MasterPageViewModel;
using System.Collections;
using System.Linq;
using System.Data;
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using oostfraeiskorg;
using System.Data.SqlTypes;
using System.Reflection.PortableExecutable;

namespace WFDOT
{
    public class Searcher
    {
        class Row
        {
            public long ID;
            public string Ostfriesisch, Deutsch, Englisch, Artikel, Nebenformen, Standardform;
            public List<string> a1 = new List<string>();
            public List<string> b1 = new List<string>();
        }

        public static IQueryable<Entry> SearchAndFill(string searchstr, string suchrichtung, string volltextsuche)
        {
            List<Entry> Entries = new List<Entry>();

            string originalSearch;
            string cellEastFrisian = "";
            string cellHeaderTranslation = "";
            string NotFound = "";
            string NeedInput = "";
            var language = Languages.German;
            string dialectaltrans = "";

            searchstr = searchstr.Trim();
            originalSearch = searchstr;

            switch (suchrichtung)
            {
                case "de>frs":
                case "frs>de":
                    cellEastFrisian = "Ostfriesisch";
                    cellHeaderTranslation = "Deutsch";
                    NotFound = $"Es sind keine Daten für die Suche '{originalSearch}' gefunden worden";
                    NeedInput = "Sie müssen mindestens ein Wort eingeben";
                    language = Languages.German;
                    dialectaltrans = "dialektal";
                    break;
                case "en>frs":
                case "frs>en":
                    cellEastFrisian = "East Frisian";
                    cellHeaderTranslation = "English";
                    NotFound = $"No data found for '{originalSearch}'";
                    NeedInput = "you have to enter at least one word";
                    language = Languages.English;
                    dialectaltrans = "dialectal";
                    break;
            }

            if (originalSearch.Equals(string.Empty))
            {
                Entries.Add(new Entry("Jī mautent minst äin wōrd ingēven", "", "", NeedInput, 0));
                return Entries.AsQueryable();
            }

            var sqlCon = DataBaseConnection.GetConnection(oostfraeiskorg.Server.MapPath(""));
            var sqlcmd = new SqliteCommand();
            sqlcmd.Connection = sqlCon;

            switch (suchrichtung.ToLower())
            {
                case "de>frs":
                    SearchStrings.DeFrs(volltextsuche, ref searchstr, ref sqlcmd);
                    break;
                case "frs>de":
                    SearchStrings.FrsDe(volltextsuche, ref searchstr, ref sqlcmd);
                    break;
                case "en>frs":
                    SearchStrings.EnFrs(volltextsuche, ref searchstr, ref sqlcmd);
                    break;
                case "frs>en":
                    SearchStrings.FrsEn(volltextsuche, ref searchstr, ref sqlcmd);
                    break;
            }

            var searchstrParamUpper = new SqliteParameter()
            {
                ParameterName = "@searchstrupper",
                Value = searchstr.ToUpper()
            };
            var searchstrParamLower = new SqliteParameter()
            {
                ParameterName = "@searchstrlower",
                Value = searchstr.ToLower()
            };
            var searchstrParam = new SqliteParameter()
            {
                ParameterName = "@searchstr",
                Value = searchstr
            };

            sqlcmd.Parameters.Add(searchstrParamUpper);
            sqlcmd.Parameters.Add(searchstrParamLower);
            sqlcmd.Parameters.Add(searchstrParam);
            sqlcmd.Prepare();
            Console.WriteLine(sqlcmd.CommandText);

            var reader = sqlcmd.ExecuteReader();
            List<Row> rows = new List<Row>();

            Console.Write("Start reading");
            while (reader.Read())
            {
                Row values = new Row();
                values.ID = reader.GetInt64("ID");
                values.Ostfriesisch = reader.GetValue("Ostfriesisch").ToString();
                values.Deutsch = reader.GetValue("Deutsch").ToString();
                values.Englisch = reader.GetValue("Englisch").ToString();
                values.Artikel = reader.GetValue("Artikel").ToString();
                values.Nebenformen = reader.GetValue("Nebenformen").ToString();
                values.Standardform = reader.GetValue("Standardform").ToString();

                try
                {
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        if (!reader.GetValue(i).ToString().Equals(string.Empty) && !reader.GetValue(i).ToString().Equals("-"))
                        {
                            if (i != 2)
                            {
                                values.a1.Add(reader.GetName(i));
                                values.b1.Add(reader.GetValue(i).ToString());
                            }
                            else
                            {
                                values.a1.Add(reader.GetName(i));
                                values.b1.Add(reader.GetValue(i).ToString());
                            }
                        }
                    }
                }
                catch
                {
                }

                rows.Add(values);
            }
            Console.Write("Finished");
            reader.Close();
            sqlCon.Close();

            var ids = new List<long>();
            var eastFrisianStrings = new List<string>();
            var eastFrisianSecondaryForms = new List<string>();
            var eastFrisianStandardForms = new List<string>();
            var eastFrisianWords = new List<string>();
            var b = new List<string>();
            foreach (var row in rows)
            {
                ids.Add(row.ID);
                var eastFrisianString = row.Ostfriesisch;
                eastFrisianString += !row.Artikel.Equals("-") ? $" ({row.Artikel})" : "";
                var eastFrisianSecondaryForm = !row.Nebenformen.Equals("-") ? $"[{dialectaltrans}: {row.Nebenformen}]" : "";
                var eastFrisianStandardForm = !row.Standardform.Equals("-") ? $"[{row.Standardform}]" : "";

                eastFrisianStrings.Add(eastFrisianString);
                eastFrisianSecondaryForms.Add(eastFrisianSecondaryForm);
                eastFrisianStandardForms.Add(eastFrisianStandardForm);
                switch (language)
                {
                    case Languages.German:
                        b.Add(row.Deutsch);
                        break;
                    case Languages.English:
                        b.Add(row.Englisch);
                        break;
                }
                eastFrisianWords.Add(row.Ostfriesisch);
            }

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i].Equals("0")) continue;

                var entry = new Entry(eastFrisianStrings[i], eastFrisianSecondaryForms[i], eastFrisianStandardForms[i], b[i], ids[i]);

                Entries.Add(entry);
            }

            if (eastFrisianStrings.Count == 0)
            {
                Entry entry = new Entry("D'r bünt ğīn dóóten föör d' söyek '" + originalSearch + "' funnen worden", "", "", NotFound, 0);
                Entries.Add(entry);
            }

            return Entries.AsQueryable();
        }

        public static string GetPopUpBody(long wordId)
        {
            string body = "";

            var sqlCon = DataBaseConnection.GetConnection(oostfraeiskorg.Server.MapPath(""));
            var sqlCommand = new SqliteCommand();
            sqlCommand.Connection = sqlCon;
            sqlCommand.CommandText = "SELECT * FROM WB Where ID ='" + wordId + "'";
            var reader = sqlCommand.ExecuteReader();

            List<string> titles = new List<string>();
            List<string> data = new List<string>();
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
            sqlCon.Close();

            List<string> notEmptyTitles = new List<string>();
            List<string> notEmptyData = new List<string>();
            for (int a = 0; a < data.Count; a++)
            {
                if (!data[a].Equals(string.Empty) && !data[a].Equals("-"))
                {
                    notEmptyTitles.Add(titles[a]);
                    notEmptyData.Add(data[a]);
                }
            }

            for (int a = 0; a < notEmptyData.Count; a++)
            {
                body += "<tr valign=\"top\"><th valign=\"top\"><p>" + notEmptyTitles[a] + ":</p></th><td valign=\"top\"><p>" + notEmptyData[a] + "</p></td ></tr>";
            }

            body = "<table class=\"table\">" + body + "</table>";
            body = body.Replace("'", "&#39;");
            body = body.Replace("\"", "&#34;");
            body = body.Replace("\r", "");
            body = body.Replace("\n", "");

            return body;
        }
    }
}