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
		result.Append("`nickname` VARCHAR(255) NOT NULL,");
		result.Append("`title` SMALLINT UNSIGNED NOT NULL,");
		result.Append("`exts` VARCHAR(2000) NOT NULL");
		result.Append(");"); // closing

		// DINO HUNTER LEADERBOARD
		result.AppendLine("CREATE TABLE IF NOT EXISTS `dh_leaderboard`");
		result.Append('('); // opening
		result.Append("`userid` VARCHAR(100) UNIQUE NOT NULL,");
		result.Append("`nickname` VARCHAR(255) NOT NULL,");
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
        /*
		// accounts
		result.AppendLine("CREATE TABLE IF NOT EXISTS `accounts`");
		result.Append("("); // opening
		result.Append("`id` BIGINT AUTO_INCREMENT PRIMARY KEY,");
		result.Append("`nick` VARCHAR(255) NOT NULL,");
		result.Append("`skin` VARCHAR(255) NOT NULL,");
		result.Append("`token` VARCHAR(255) NOT NULL,");
		result.Append("`device` VARCHAR(255) NOT NULL,");
		result.Append("`rank` INT NOT NULL DEFAULT '0',");
		result.Append("`paying` BOOLEAN NOT NULL DEFAULT '0',");
		result.Append("`developer` BOOLEAN NOT NULL DEFAULT '0',");
		result.Append("`banned` BOOLEAN NOT NULL DEFAULT 0,");
		result.Append("`RatingDeathmatch` INT NOT NULL DEFAULT '0',");
		result.Append("`RatingTeamBattle` INT NOT NULL DEFAULT '0',");
		result.Append("`RatingHunger` INT NOT NULL DEFAULT '0',");
		result.Append("`RatingCapturePoint` INT NOT NULL DEFAULT '0'");
		result.Append(");"); // closing

		// badfilter
		result.AppendLine("CREATE TABLE IF NOT EXISTS `badfilter`(`value` tinytext NOT NULL);");

		// config
		result.AppendLine("CREATE TABLE IF NOT EXISTS `config`");
		result.Append("("); // opening
		result.Append("`key` VARCHAR(255) NOT NULL,");
		result.Append("`value` VARCHAR(255) NOT NULL");
		result.Append(");"); // closing

		// friend_requests
		result.AppendLine("CREATE TABLE IF NOT EXISTS `friend_requests`");
		result.Append("("); // opening
		result.Append("`from` BIGINT NOT NULL,");
		result.Append("`to` BIGINT NOT NULL");
		result.Append(");"); // closing
		*/

        return result.ToString();
	}
}