using MySqlConnector;
using T.Objects;

namespace T.Db;

public class DinoHunterDB
{
	public static async Task SaveUser(DinoHunterAccount account)
	{
		using var db = await DatabaseManager.GetOpen();

		const string sql = @"
		INSERT INTO `dh_accounts` (userid, nickname, title, exts)
		VALUES (@userid, @nickname, @title, @exts)
		ON DUPLICATE KEY UPDATE 
			nickname = VALUES(nickname),
			title = VALUES(title),
			exts = VALUES(exts);";

		using var cmd = new MySqlCommand(sql, db);

		cmd.Parameters.AddWithValue("@userid", account.userId);
		cmd.Parameters.AddWithValue("@nickname", account.nickname);
		cmd.Parameters.AddWithValue("@title", account.title);
		cmd.Parameters.AddWithValue("@exts", account.exts);

		var rowsAffected = await cmd.ExecuteNonQueryAsync();

		Debug.Log($"Updated info for user: {account.userId}. Rows Affected: {rowsAffected}.");
	}

	public static async Task<DinoHunterAccount?> LoadUser(string userId)
	{
		using var db = await DatabaseManager.GetOpen();

		const string sql = @"
			SELECT userid, nickname, title, exts
			FROM dh_accounts
			WHERE userid = @userid;";

		using var cmd = new MySqlCommand(sql, db);
		cmd.Parameters.AddWithValue("@userid", userId);

		using var reader = await cmd.ExecuteReaderAsync();

		if (await reader.ReadAsync())
		{
			return new DinoHunterAccount
			{
				userId = reader["userid"].ToString(),
				nickname = reader["nickname"].ToString(),
				title = Convert.ToInt32(reader["title"]),
				exts = reader["exts"].ToString()
			};
		}

		return null;
	}

	public static async Task InsertLeaderboard(DinoHunterAccount account)
	{
		using var db = await DatabaseManager.GetOpen();

		const string sql = @"
		INSERT INTO `dh_leaderboard` (userid, nickname, combatpower, exp, hunterLv, crystal, gold, applause)
		VALUES (@userid, @nickname, @combatpower, @exp, @hunterLv, @crystal, @gold, @applause)
		ON DUPLICATE KEY UPDATE 
			nickname = VALUES(nickname),
			combatpower = VALUES(combatpower),
			exp = VALUES(exp),
			hunterLv = VALUES(hunterLv),
			crystal = VALUES(crystal),
			gold = VALUES(gold),
			applause = VALUES(applause);";

		using var cmd = new MySqlCommand(sql, db);

		cmd.Parameters.AddWithValue("@userid", account.userId);
		cmd.Parameters.AddWithValue("@nickname", account.nickname);
		cmd.Parameters.AddWithValue("@combatpower", account.combatpower);
		cmd.Parameters.AddWithValue("@exp", account.exp);
		cmd.Parameters.AddWithValue("@hunterLv", account.hunterLv);
		cmd.Parameters.AddWithValue("@crystal", account.crystal);
		cmd.Parameters.AddWithValue("@gold", account.gold);
		cmd.Parameters.AddWithValue("@applause", account.applause);

		var rowsAffected = await cmd.ExecuteNonQueryAsync();

		//Debug.Log($"Updated leaderboard for user: {account.userId}. Rows Affected: {rowsAffected}.");
	}

	public async static Task<List<DinoHunterAccount>> ListLeaderboard()
	{
		using var db = await DatabaseManager.GetOpen();

        using var cmd = new MySqlCommand($@"
			SELECT * FROM `dh_leaderboard` 
			ORDER BY hunterLv DESC, exp DESC, combatpower DESC 
			LIMIT {Config.dhLeaderboardReturnAmount};", db);

        using var reader = await cmd.ExecuteReaderAsync();

		var result = new List<DinoHunterAccount>();

		while (await reader.ReadAsync())
		{
			result.Add(new DinoHunterAccount()
			{
				userId = reader["userid"].ToString(),
				nickname = reader["nickname"].ToString(),
				combatpower = Convert.ToInt32(reader["combatpower"]),
				exp = Convert.ToInt32(reader["exp"]),
				hunterLv = Convert.ToInt32(reader["hunterLv"]),
				crystal = Convert.ToInt32(reader["crystal"]),
				gold = Convert.ToInt32(reader["gold"]),
				applause = Convert.ToInt32(reader["applause"])
			});
		}

		return result;
	}

	public async static Task<int> GetPlaceFor(string userId)
	{
		using var db = await DatabaseManager.GetOpen();

		const string sql = @"
		SELECT COUNT(*) + 1 AS place
		FROM dh_leaderboard
		WHERE hunterLv > (
			SELECT hunterLv FROM dh_leaderboard WHERE userid = @userId
		);";

		using var cmd = new MySqlCommand(sql, db);
		cmd.Parameters.AddWithValue("@userId", userId);

		var result = await cmd.ExecuteScalarAsync();

		if (result != null && int.TryParse(result.ToString(), out int rank))
			return rank;

		return 0;
	}
}
