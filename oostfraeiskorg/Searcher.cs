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

namespace WFDOT
{
    public class Searcher
    {
        class Row
        {
            public long ID;
            public string Ostfriesisch, Deutsch, Englisch, Artikel, Nebenformen, Standardform;
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
                Entries.Add(new Entry("Jī mautent minst äin wōrd ingēven", "", "", NeedInput));
                return Entries.AsQueryable();
            }

            var sqlCon = SQLCON.GetConnection(oostfraeiskorg.Server.MapPath(""));
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
                Console.Write(".");
                Row values = new Row();
                values.ID = reader.GetInt64("ID");
                values.Ostfriesisch = reader.GetValue("Ostfriesisch").ToString();
                values.Deutsch = reader.GetValue("Deutsch").ToString();
                values.Englisch = reader.GetValue("Englisch").ToString();
                values.Artikel = reader.GetValue("Artikel").ToString();
                values.Nebenformen = reader.GetValue("Nebenformen").ToString();
                values.Standardform = reader.GetValue("Standardform").ToString();
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
                Entry entry = new Entry(eastFrisianStrings[i], eastFrisianSecondaryForms[i], eastFrisianStandardForms[i], b[i]);
                /*resultCellFrisian.Text = eastFrisianStrings[i];
                resultCellTranslation.Text = b[i];
                if (ids[i].Equals("0"))
                {
                    resultCellFrisian.Font.Italic = true;
                    resultCellFrisian.BackColor = ColorTranslator.FromHtml("#99CCFF");
                    resultCellTranslation.Font.Italic = true;
                    resultCellTranslation.BackColor = ColorTranslator.FromHtml("#99CCFF");
                    resultCellInfo.Text = "";
                    resultCellInfo.BackColor = ColorTranslator.FromHtml("#99CCFF");
                }
                else
                {
                    var bud = new DetailButton
                    {
                        CssClass = "btn btn_secondary bg-primary",
                        ImageUrl = "/img/info.png"
                    };
                    bud.Click += new ImageClickEventHandler(page.showPopup);
                    bud.wordid = ids[i];
                    bud.ToolTip = eastFrisianWords[i];
                    resultCellInfo.Controls.Add(bud);

                    string mp3 = $"{page.Server.MapPath(" / ")}rec/{eastFrisianWords[i]}.mp3";
                    if (File.Exists(mp3))
                    {
                        var bus = new CustomImageButton
                        {
                            CssClass = "btn btn_secondary bg-primary",
                            ImageUrl = "/img/sound.png"
                        };
                        bus.Click += new ImageClickEventHandler(page.soundButton);
                        bus.word = eastFrisianWords[i];
                        bus.ToolTip = eastFrisianWords[i];
                        resultCellInfo.Controls.Add(bus);
                    }
                }
                resultRow.Cells.Add(resultCellFrisian);
                resultRow.Cells.Add(resultCellTranslation);
                resultRow.Cells.Add(resultCellInfo);*/
                Entries.Add(entry);
            }
            if (eastFrisianStrings.Count == 0)
            {
                Entry entry = new Entry("D'r bünt ğīn dóóten föör d' söyek '" + originalSearch + "' funnen worden", "", "", NotFound);
                Entries.Add(entry);
            }

            /*switch (suchrichtung)
            {
                case "de>frs":
                case "frs>de":
                    page.Master.Page.Title = $"Suche nach {originalSearch}({page.df}) - Ōstfräisk wōrdenbauk - Ostfriesisches Wörterbuch";
                    page.Master.Page.MetaDescription = $"Übersetzung für {originalSearch}({page.df}) auf Ostfriesisch - Wörterbuch der ostfriesischen Sprache";
                    break;
                case "en>frs":
                case "frs>en":
                    page.Master.Page.Title = $"Searched for {originalSearch}({page.df}) - Ōstfräisk wōrdenbauk - East Frisian Dictionary";
                    page.Master.Page.MetaDescription = $"Translation for {originalSearch}({page.df}) into East Frisian - Dictionary of the East Frisian Language";
                    break;
            }*/
            return Entries.AsQueryable();
        }
    }
}