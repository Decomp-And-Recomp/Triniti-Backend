using T.Database;
using T.MySql;
using T.Logging;
using T.SQLite;

namespace T;

public static class DB
{
    public static DatabaseController current = null!;

    public static BanDatabase banDatabase => current.banDatabase;
    public static DinoHunterDatabase dinoHunterDatabase => current.dinoHunterDatabase;
    public static GameConfigDatabase gameConfigDatabase => current.gameConfigDatabase;

    public static Task Init()
    {
        switch (Config.Database.type)
        {
            case Config.Database.Type.SQLite: current = new SQLiteDatabase(); break;
            case Config.Database.Type.MySQL: current = new MySqlDatabase(); break;
        }

        current.logger.onLog = OnLog;

        return current.Initialize(Config.Database.server, Config.Database.port, Config.Database.databaseName, Config.Database.userId, Config.Database.password);
    }

    static void OnLog(DatabaseLogger.LogLevel level, string log)
    {
        Logger.Log((Logging.LogLevel)level, log); // simple conversion works because enums are the same.
    }
}