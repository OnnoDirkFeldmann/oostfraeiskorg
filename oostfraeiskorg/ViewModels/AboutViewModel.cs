using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace oostfraeiskorg.ViewModels;

public class AboutViewModel : MasterPageViewModel
{
    public string DataCount
    {
        get
        {
            string ls_ds = "";
            var sqlCon = DataBaseConnection.GetConnection(oostfraeiskorg.Server.MapPath(""));
            var sqlcmd = new SqliteCommand();
            sqlcmd.Connection = sqlCon;
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

    public override Task Init()
    {
        MasterPageTitle = "Über - Oostfräisk Woordenbauk - Ostfriesisches Wörterbuch";
        MasterPageDescription = "Über - Wörterbuch der ostfriesischen Sprache - Wörter aus dem Ostfriesischen oder ins Ostfriesische übersetzen. Die Sprache der Ostfriesen mit dem Wörterbuch für das ostfriesische Platt als Standardostfriesisch lernen.";
        MasterPageKeywords += ", über, about, jungfräiske mäinskup, jfm, ostfriesische Sprache, ostfriesisch, oostfräisk, links";
        return base.Init();
    }

}

