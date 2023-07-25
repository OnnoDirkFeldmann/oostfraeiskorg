using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace WFDOT
{
    public partial class main : System.Web.UI.Page
    {
        public string suchstring;
        public string df;
        public string fts;
        protected void Page_Load(object sender, EventArgs e)
        {
            suchstring = Request.QueryString["W"];
            df = Request.QueryString["df"];
            fts = Request.QueryString["fts"];

            //Alte URL-Versionen händeln
            if (df == "ofrs" || df == "frs")
            {
                df = "frs>de";
            }
            if (df == "de")
            {
                df = "de>frs";
            }

            //Default Sprache
            if (df != "de>frs" && df != "frs>de" && df != "en>frs" && df != "frs>en")
            {
                df = "de>frs";
            }

            //Default Search
            if (fts != "J" && fts != "N" && fts != "X")
            {
                fts = "N";
            }
            if (suchstring != null)
            {
                if (suchstring.Contains("%"))
                {
                    Response.Redirect("~/main.aspx");
                }
                else
                {
                    new Searcher().SearchAndFill(suchstring, df, fts, tbResult, this);
                }
            }
            else
            {
                Master.Page.Title = "Ōstfräisk wōrdenbauk - Ostfriesisches Wörterbuch";
            }
        }


        public void showPopup(object sender, EventArgs e)
        {
            long wortid = (sender as DetailButton).wordid;
            string title = "Details";
            string body = "";

            var sqlCon = SQLCON.GetConnection(Server.MapPath("/"));
            SqliteCommand sqlcmd = new SqliteCommand(sqlCon);
            sqlcmd.CommandText = "SELECT * FROM WB Where ID ='" + wortid + "'";
            SqliteDataReader reader = sqlcmd.ExecuteReader();

            List<string> a1 = new List<string>();
            List<string> b1 = new List<string>();
            while (reader.Read())
            {
                try
                {
                    int i = 1;
                    while (true)
                    {
                        if (i != 2)
                        {
                            a1.Add(reader.GetName(i));
                            b1.Add(reader.GetValue(i).ToString());
                        }
                        else
                        {
                            a1.Add(reader.GetName(i));
                            b1.Add(reader.GetValue(i).ToString());
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

            List<string> a2 = new List<string>();
            List<string> b2 = new List<string>();
            int y = 0;
            for (int i = 0; i < b1.Count; i++)
            {
                if (!b1[i].Equals(string.Empty) && !b1[i].Equals("-"))
                {
                    a2.Add(a1[i]);
                    b2.Add(b1[i]);
                    y++;
                }
            }

            for (int i = 0; i < b2.Count; i++)
            {
                body += "<tr valign=\"top\"><th valign=\"top\"><p>" + a2[i] + ":</p></th><td valign=\"top\"><p>" + b2[i] + "</p></td ></tr>";
            }

            body = "<table class=\"table\">" + body + "</table>";
            body = body.Replace("'", "&#39;");
            body = body.Replace("\"", "&#34;");
            body = body.Replace("\r", "");
            body = body.Replace("\n", "");

            ClientScript.RegisterStartupScript(GetType(), "ShowPopup", "ShowPopup('" + title + "', '" + body + "');", true);
        }

        public void soundButton(object sender, EventArgs e)
        {
            string word = (sender as CustomImageButton).word;
            playSound("/rec/" + word + ".mp3");
        }
        public void playSound(string audio)
        {
            string js;
            js = "<script type='text/javascript'>var sound = new Audio('" + audio + "');sound.play();</script>";
            ClientScript.RegisterStartupScript(GetType(), "PlayWbSound", js);
        }

    }
}