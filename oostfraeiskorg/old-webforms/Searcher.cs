using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WFDOT
{
    public class Searcher
    {
        public void SearchAndFill(string searchstr, string suchrichtung, string volltextsuche, Table tbResult, main page)
        {
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
                var row = new TableRow();
                row.CssClass = "row";
                var cellFrisian = new TableCell();
                var cellTranslation = new TableCell();
                cellFrisian.Text = "Jī mautent minst äin wōrd ingēven";
                cellTranslation.Text = NeedInput;
                row.Cells.Add(cellFrisian);
                row.Cells.Add(cellTranslation);
                tbResult.Rows.Add(row);
                return;
            }

            var sqlCon = SQLCON.GetConnection(page.Server.MapPath("/"));
            var sqlcmd = new SqliteCommand(sqlCon);
            var searchstrParamUpper = new SqliteParameter("@searchstrupper");
            var searchstrParamLower = new SqliteParameter("@searchstrlower");
            var searchstrParam = new SqliteParameter("@searchstr");

            switch (suchrichtung)
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

            searchstrParamUpper.Value = searchstr.ToUpper();
            searchstrParamLower.Value = searchstr.ToLower();
            searchstrParam.Value = searchstr;
            sqlcmd.Parameters.Add(searchstrParamUpper);
            sqlcmd.Parameters.Add(searchstrParamLower);
            sqlcmd.Parameters.Add(searchstrParam);
            sqlcmd.Prepare();

            var reader = sqlcmd.ExecuteReader();
            var wfdot = new WFDOT();
            wfdot._WFDOT.Load(reader);
            reader.Close();
            sqlCon.Close();

            var ids = new List<long>();
            var eastFrisianStrings = new List<string>();
            var eastFrisianWords = new List<string>();
            var b = new List<string>();
            foreach (var row in wfdot._WFDOT)
            {
                ids.Add(row.ID);
                var eastFrisianString = row.Ostfriesisch;
                eastFrisianString += !row.Artikel.Equals("-") ? $" ({row.Artikel})" : "";
                eastFrisianString += !row.Nebenformen.Equals("-") ? $"<br/>[{dialectaltrans}: {row.Nebenformen}]" : "";
                eastFrisianString += !row.Standardform.Equals("-") ? $"<br/>[{row.Standardform}]" : "";
                eastFrisianStrings.Add(eastFrisianString);
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
            var headerRow = new TableRow();
            var headerCellFrisian = new TableCell();
            headerCellFrisian.Text = cellEastFrisian;
            headerCellFrisian.Font.Bold = true;
            headerCellFrisian.BackColor = ColorTranslator.FromHtml("#99CCFF");
            headerRow.Cells.Add(headerCellFrisian);

            var headerCellTranslation = new TableCell();
            headerCellTranslation.Text = cellHeaderTranslation;
            headerCellTranslation.Font.Bold = true;
            headerCellTranslation.BackColor = ColorTranslator.FromHtml("#99CCFF");
            headerRow.Cells.Add(headerCellTranslation);

            var headerCellInfo = new TableCell();
            headerCellInfo.Text = "";
            headerCellInfo.Font.Bold = true;
            headerCellInfo.BackColor = ColorTranslator.FromHtml("#99CCFF");
            headerRow.Cells.Add(headerCellInfo);

            tbResult.Rows.Add(headerRow);
            headerRow.Font.Bold = true;
            for (int i = 0; i < ids.Count; i++)
            {
                var resultRow = new TableRow();
                var resultCellFrisian = new TableCell();
                var resultCellTranslation = new TableCell();
                var resultCellInfo = new TableCell();
                resultCellFrisian.Text = eastFrisianStrings[i];
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
                resultRow.Cells.Add(resultCellInfo);
                tbResult.Rows.Add(resultRow);
            }
            if (eastFrisianStrings.Count == 0)
            {
                var row = new TableRow();
                var celltmp1 = new TableCell();
                var celltmp2 = new TableCell();
                var celltmp3 = new TableCell();
                celltmp1.Text = $"D'r bünt ğīn dóóten föör d' söyek '{originalSearch}' funnen worden";
                celltmp2.Text = NotFound;
                celltmp3.Text = "";
                row.Cells.Add(celltmp1);
                row.Cells.Add(celltmp2);
                row.Cells.Add(celltmp3);
                tbResult.Rows.Add(row);
            }

            switch (suchrichtung)
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
            }

        }
    }
}