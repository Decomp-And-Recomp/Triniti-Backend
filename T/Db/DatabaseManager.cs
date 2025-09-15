using MySqlConnector;

namespace T.Db;

internal class DatabaseManager
{
    public static async Task Init()
    {
        using var conn = await GetOpen();

        using MySqlCommand command = new(GetInitCommands(), conn);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>Get raw, not open connection with <see cref="Settings.mySqlConnectionString"/>, USE `using` TO DISPOSE AFTER USING!</summary>
    public static MySqlConnection Get()
    {
        //ToDo: make it use Config.mySqlConnectionString without specifying it every time i start connection?
        // (Set it as default in db settings on server start?)
        return new MySqlConnection(Config.mySqlConnectionString);
    }

    /// <summary>Get open connection with <see cref="Config.mySqlConnectionString"/>, USE `using` TO DISPOSE AFTER USING!</summary>
    public static async Task<MySqlConnection> GetOpen()
    {
        MySqlConnection conn = Get();

        await conn.OpenAsync();

        return conn;
    }

    // yes ill do it later
    static string GetInitCommands()
    {
        var result = new System.Text.StringBuilder();

        // DINO HUNTER ACCOUNTS
        result.AppendLine("CREATE TABLE IF NOT EXISTS `dh_accounts`");
        result.Append('('); // opening
        result.Append("`userid` VARCHAR(100) UNIQUE NOT NULL,");
        result.Append("`nickname` VARCHAR(12) NOT NULL,");
        result.Append("`title` SMALLINT UNSIGNED NOT NULL,");
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

        result.AppendLine("CREATE TABLE IF NOT EXISTS `filter`");
        result.Append('('); // opening
        result.Append("`badword` VARCHAR(12) NOT NULL");
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

        return result.ToString();
    }
}