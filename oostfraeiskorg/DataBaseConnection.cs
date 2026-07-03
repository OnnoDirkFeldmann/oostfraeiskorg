using Microsoft.Data.Sqlite;

namespace oostfraeiskorg;

public class DataBaseConnection
{
    public static SqliteConnection GetConnection(string startpath)
    {
        var sqliteConnection = new SqliteConnection();
        string dataSource = startpath + @"/WFDOT.db";
        sqliteConnection.ConnectionString = $"Data Source={dataSource}; Mode=ReadOnly; Cache=Shared; Pooling=True;";
        sqliteConnection.Open();
        return sqliteConnection;
    }
}
