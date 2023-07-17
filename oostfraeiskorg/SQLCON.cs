
using Microsoft.Data.Sqlite;

namespace WFDOT
{
    public class SQLCON
    {
        public static SqliteConnection GetConnection(string startpath)
        {
            SqliteConnection connection = new SqliteConnection();
            string dataSource = startpath + @"WFDOT.db";
            connection.ConnectionString = $"Data Source={dataSource}; Pooling=False;";
            connection.Open();
            return connection;
        }
    }
}