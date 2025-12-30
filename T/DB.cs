using T.Database;
using T.MySql;
using T.Logging;

namespace T;

public static class DB
{
    public static DatabaseController Current = null!;

    public static BanDatabase BanDatabase => Current.banDatabase;
    public static DinoHunterDatabase DinoHunterDatabase => Current.dinoHunterDatabase;
    public static GameConfigDatabase GameConfigDatabase => Current.gameConfigDatabase;

    public static Task Init()
    {
        switch (Config.Database.Type)
        {
            case Config.Database.DatabaseType.MySQL: Current = new MySqlDatabase(); break;
        }

        Current.logger.onLog = OnLog;

        return Current.Initialize(Config.Database.Server, Config.Database.Port, Config.Database.DatabaseName, Config.Database.UserId, Config.Database.Password);
    }

    private static void OnLog(DatabaseLogger.LogLevel level, string log)
    {
        Logger.Log((Logging.LogLevel)level, log); // simple conversion works because enums are the same.
    }
}