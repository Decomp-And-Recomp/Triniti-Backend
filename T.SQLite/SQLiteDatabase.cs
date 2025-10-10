using T.Database;

namespace T.SQLite;

public class SQLiteDatabase : DatabaseController
{
    public override Task Initialize(string server, int port, string database, string user, string password)
    {
        return Task.CompletedTask;
    }
}
