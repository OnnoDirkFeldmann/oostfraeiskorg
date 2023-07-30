
using Microsoft.Data.Sqlite;
using System;

namespace WFDOT
{
    public class SQLCON
    {
        public static SqliteConnection GetConnection(string startpath)
        {
            SqliteConnection connection = new SqliteConnection();
            string dataSource = startpath + @"\WFDOT.db";
            connection.ConnectionString = $"Data Source={dataSource}; Pooling=False;";
            Console.WriteLine(connection.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}