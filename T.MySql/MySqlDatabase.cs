using MySqlConnector;
using T.Database;

namespace T.MySql;

public class MySqlDatabase : DatabaseController
{
    string connectionString = string.Empty;

    public MySqlDatabase()
    {
        banDatabase = new MySqlBanDatabase(this);
        dinoHunterDatabase = new MySqlDinoHunterDatabase(this);
        gameConfigDatabase = new MySqlGameConfigDatabase(this);
    }

    public override async Task Initialize(string server, int port, string database, string user, string password)
    {
        connectionString = $"Server={server};Port={port};User ID={user};Password={password};Database={database}";

        using var conn = await GetOpen();

        using MySqlCommand command = new(GetInitCommands(), conn);
        await command.ExecuteNonQueryAsync();
    }

    MySqlConnection Get()
    {
        return new MySqlConnection(connectionString);
    }

    /// <summary>Get new open connection, USE `using` TO DISPOSE AFTER!</summary>
    internal async Task<MySqlConnection> GetOpen()
    {
        MySqlConnection conn = Get();

        await conn.OpenAsync();

        return conn;
    }

    static string GetInitCommands()
    {
        var result = new System.Text.StringBuilder();

        // DINO HUNTER ACCOUNTS
        result.AppendLine("CREATE TABLE IF NOT EXISTS `dh_accounts`");
        result.Append('('); // opening
        result.Append("`userid` VARCHAR(100) UNIQUE NOT NULL,");
        result.Append("`nickname` VARCHAR(12) NOT NULL,");
        result.Append("`title` INT NOT NULL,");
        result.Append("`exts` VARCHAR(1000) NOT NULL,");
        result.Append("`ip` VARCHAR(64) NOT NULL");
        result.Append(");"); // closing

        // DINO HUNTER LEADERBOARD
        result.AppendLine("CREATE TABLE IF NOT EXISTS `dh_leaderboard`");
        result.Append('('); // opening
        result.Append("`userid` VARCHAR(100) UNIQUE NOT NULL,");
        result.Append("`nickname` VARCHAR(12) NOT NULL,");
        result.Append("`combatpower` INT NOT NULL,");
        result.Append("`exp` INT NOT NULL,");
        result.Append("`hunterLv` INT NOT NULL,");
        result.Append("`crystal` INT NOT NULL,");
        result.Append("`gold` INT NOT NULL,");
        result.Append("`applause` INT NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `ip_bans`");
        result.Append('('); // opening
        result.Append("`ip` VARCHAR(64) NOT NULL,");
        result.Append("`reason` VARCHAR(255) NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `hwid_bans`");
        result.Append('('); // opening
        result.Append("`hwid` VARCHAR(100) NOT NULL,");
        result.Append("`reason` VARCHAR(255) NOT NULL");
        result.Append(");"); // closing

        result.AppendLine("CREATE TABLE IF NOT EXISTS `game_config`");
        result.Append('('); // opening
        result.Append("`game` VARCHAR(24) UNIQUE NOT NULL,");
        result.Append("`ip` VARCHAR(255) NOT NULL,");
        result.Append("`port` INT NOT NULL,");
        result.Append("`version` VARCHAR(255) NOT NULL");
        result.Append(");"); // closing

        return result.ToString();
    }
}
