using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using WFDOT;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace oostfraeiskorg.ViewModels
{
    public class MainViewModel : MasterPageViewModel
    {

        public MainViewModel()
		{
		}

        public void showPopup(long wordid)
        {
            title = "Details";
            body = "";

            var sqlCon = SQLCON.GetConnection(oostfraeiskorg.Server.MapPath(""));
            var sqlcmd = new SqliteCommand();
            sqlcmd.Connection = sqlCon;

            sqlcmd.CommandText = "SELECT * FROM WB Where ID = @wordid";

            var wordidParam = new SqliteParameter()
            {
                ParameterName = "@wordid",
                Value = wordid
            };

            sqlcmd.Parameters.Add(wordidParam);
            sqlcmd.Prepare();
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
        }

    }
}
