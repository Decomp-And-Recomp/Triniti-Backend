using System.Data.SQLite;
using T.Database;

namespace T.SQLite;

public class SQLiteDatabase : DatabaseController
{
    string path = string.Empty;
    string connectionString = string.Empty;

    public override async Task Initialize(string server, int port, string database, string user, string password)
    {
        throw new NotImplementedException();
#pragma warning disable
        logger.Log(DatabaseLogger.LogLevel.Info, "SQLite: Initializing.");

        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Triniti-Backend", "sqlite.db");

        string dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        logger.Log(DatabaseLogger.LogLevel.Info, $" SQLite: The database file will be located at '{path}'.");
        logger.Log(DatabaseLogger.LogLevel.Info, $" SQLite: Inititalizing database file...");

        connectionString = $"Data Source={path};Cache=Shared;Pooling=True;";

        using var conn = await GetOpen();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = GetInitCommands();

        await cmd.ExecuteNonQueryAsync();

        logger.Log(DatabaseLogger.LogLevel.Info, $" SQLite: Database initialized.");
    }

    SQLiteConnection Get()
    {
        return new SQLiteConnection(connectionString);
    }

    /// <summary>Get new open connection, USE `using` TO DISPOSE AFTER!</summary>
    async Task<SQLiteConnection> GetOpen()
    {
        var conn = Get();
        await conn.OpenAsync();
        return conn;
    }

    static string GetInitCommands()
    {
        var result = new System.Text.StringBuilder();

        // DINO HUNTER ACCOUNTS
        result.AppendLine("CREATE TABLE IF NOT EXISTS `dh_accounts`");
        result.Append('('); // opening
        result.Append("`userid` TEXT UNIQUE NOT NULL,");
        result.Append("`nickname` TEXT NOT NULL,");
        result.Append("`title` INT NOT NULL,");
        result.Append("`exts` TEXT NOT NULL,");
        result.Append("`ip` TEXT NOT NULL");
        result.Append(");"); // closing

        // DINO HUNTER LEADERBOARD
        result.AppendLine("CREATE TABLE IF NOT EXISTS `dh_leaderboard`");
        result.Append('('); // opening
        result.Append("`userid` TEXT UNIQUE NOT NULL,");
        result.Append("`nickname` TEXT NOT NULL,");
        result.Append("`combatpower` INT NOT NULL,");
        result.Append("`exp` INT NOT NULL,");
        result.Append("`hunterLv` INT NOT NULL,");
        result.Append("`crystal` INT NOT NULL,");
        result.Append("`gold` INT NOT NULL,");
        result.Append("`applause` INT NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `filter`");
        result.Append('('); // opening
        result.Append("`badword` TEXT NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `ip_bans`");
        result.Append('('); // opening
        result.Append("`ip` TEXT NOT NULL,");
        result.Append("`reason` TEXT NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `hwid_bans`");
        result.Append('('); // opening
        result.Append("`hwid` TEXT NOT NULL,");
        result.Append("`reason` TEXT NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `game_config`");
        result.Append('('); // opening
        result.Append("`game` TEXT UNIQUE NOT NULL,");
        result.Append("`ip` TEXT NOT NULL,");
        result.Append("`port` INT NOT NULL");
        result.Append("`version` TEXT NOT NULL");
        result.Append(");"); // closing

        return result.ToString();
    }
}
