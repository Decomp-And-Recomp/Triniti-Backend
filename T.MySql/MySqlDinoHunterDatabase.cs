using MySqlConnector;
using T.Database;
using T.Database.Objects.DinoHunter;

namespace T.MySql;

public class MySqlDinoHunterDatabase : DinoHunterDatabase
{
    protected readonly MySqlDatabase controller;

    public MySqlDinoHunterDatabase(MySqlDatabase controller)
    {
        this.controller = controller;
    }

    public override async Task SaveUser(AccountEntry entry)
    {
        using var db = await controller.GetOpen();

        const string sql = @"
        INSERT INTO `dh_accounts` (userid, nickname, title, exts, ip)
        VALUES (@userid, @nickname, @title, @exts, @ip)
        ON DUPLICATE KEY UPDATE 
            nickname = VALUES(nickname),
            title = VALUES(title),
            exts = VALUES(exts),
            ip = VALUES(ip);";

        using var cmd = new MySqlCommand(sql, db);

        cmd.Parameters.AddWithValue("@userid", entry.userId);
        cmd.Parameters.AddWithValue("@nickname", entry.nickname);
        cmd.Parameters.AddWithValue("@title", entry.title);
        cmd.Parameters.AddWithValue("@exts", entry.exts);
        cmd.Parameters.AddWithValue("@ip", entry.ip);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
    }

    public override async Task<AccountEntry?> LoadUser(string? userId)
    {
        using var db = await controller.GetOpen();

        const string sql = @"
            SELECT userid, nickname, title, exts
            FROM dh_accounts
            WHERE userid = @userid;";

        using var cmd = new MySqlCommand(sql, db);
        cmd.Parameters.AddWithValue("@userid", userId);

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync()) return null;

        return new AccountEntry
        {
            userId = reader["userid"].ToString(),
            nickname = reader["nickname"].ToString(),
            title = Convert.ToInt32(reader["title"]),
            exts = reader["exts"].ToString()
        };
    }
    public override async Task InsertLeaderboard(LeaderboardEntry entry)
    {
        using var db = await controller.GetOpen();

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

        cmd.Parameters.AddWithValue("@userid", entry.userId);
        cmd.Parameters.AddWithValue("@nickname", entry.nickname);
        cmd.Parameters.AddWithValue("@combatpower", entry.combatpower);
        cmd.Parameters.AddWithValue("@exp", entry.exp);
        cmd.Parameters.AddWithValue("@hunterLv", entry.hunterLv);
        cmd.Parameters.AddWithValue("@crystal", entry.crystal);
        cmd.Parameters.AddWithValue("@gold", entry.gold);
        cmd.Parameters.AddWithValue("@applause", entry.applause);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
    }

    public override async Task<List<LeaderboardEntry>> ListLeaderboard(int amount)
    {
        using var db = await controller.GetOpen();

        using var cmd = new MySqlCommand($@"
            SELECT * FROM `dh_leaderboard` 
            ORDER BY hunterLv DESC, exp DESC, combatpower DESC 
            LIMIT {amount};", db);

        using var reader = await cmd.ExecuteReaderAsync();

        var result = new List<LeaderboardEntry>();

        while (await reader.ReadAsync())
        {
            result.Add(new LeaderboardEntry()
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

    public override async Task<LeaderboardEntry?> FromLeaderboard(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;

        using var db = await controller.GetOpen();

        const string sql = @"
            SELECT * FROM dh_leaderboard
            WHERE userid = @userid;";

        using var cmd = new MySqlCommand(sql, db);

        cmd.Parameters.AddWithValue("@userid", userId);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new LeaderboardEntry
            {
                userId = reader["userid"].ToString(),
                nickname = reader["nickname"].ToString(),
                combatpower = Convert.ToInt32(reader["combatpower"]),
                exp = Convert.ToInt32(reader["exp"]),
                hunterLv = Convert.ToInt32(reader["hunterLv"]),
                crystal = Convert.ToInt32(reader["crystal"]),
                gold = Convert.ToInt32(reader["gold"]),
                applause = Convert.ToInt32(reader["applause"])
            };
        }

        return null;
    }

    public override async Task<int> GetPlaceFor(string? userId)
    {
        if (userId == null) return -1;

        using var db = await controller.GetOpen();

        const string sql = @"
        SELECT COUNT(*) + 1 AS place
        FROM dh_leaderboard
        WHERE 
            (hunterLv > (SELECT hunterLv FROM dh_leaderboard WHERE userid = @userId))
            OR (
                hunterLv = (SELECT hunterLv FROM dh_leaderboard WHERE userid = @userId)
                AND exp > (SELECT exp FROM dh_leaderboard WHERE userid = @userId)
            )
            OR (
                hunterLv = (SELECT hunterLv FROM dh_leaderboard WHERE userid = @userId)
                AND exp = (SELECT exp FROM dh_leaderboard WHERE userid = @userId)
                AND combatpower > (SELECT combatpower FROM dh_leaderboard WHERE userid = @userId)
            );";

        using var cmd = new MySqlCommand(sql, db);
        cmd.Parameters.AddWithValue("@userId", userId);

        var result = await cmd.ExecuteScalarAsync();

        if (result != null && int.TryParse(result.ToString(), out int rank))
            return rank;

        return 0;
    }
}
