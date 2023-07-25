using Mono.Data.Sqlite;
using System;

namespace WFDOT
{
    public partial class about : System.Web.UI.Page
    {
        public string DataCount
        {
            get{
                string ls_ds = "";
                var sqlCon = SQLCON.GetConnection(Server.MapPath("/"));
                var sqlcmd = new SqliteCommand(sqlCon);
                sqlcmd.CommandText = "SELECT COUNT(*) FROM WB";
                SqliteDataReader reader = sqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    ls_ds = reader.GetValue(0).ToString();
                }
                reader.Close();
                sqlCon.Close();
                return ls_ds;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Page.Title = "Über - Ōstfräisk Wōrdenbauk - Ostfriesisches Wörterbuch";
            Master.Page.MetaKeywords += "über, about, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, ōstfräisk, links";
            Master.Page.MetaDescription = "Über - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";

        }
    }
}