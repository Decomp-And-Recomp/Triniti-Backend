using MySqlConnector;

namespace T.Db;

public static class BanDB
{
    public static async Task<bool> IsIpBanned(string? ip)
    {
        if (string.IsNullOrEmpty(ip)) return true;

        using var conn = await DatabaseManager.GetOpen();

        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM ip_bans WHERE ip = @ip", conn);
        cmd.Parameters.AddWithValue("@ip", ip);

        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return result > 0;
    }

    public static async Task<bool> IsIpBanned(HttpContext context)
        => await IsIpBanned(Utils.GetIp(context));

    public static async Task<bool> IsHWIDBanned(string? hwid)
    {
        if (string.IsNullOrEmpty(hwid)) return true;

        using var conn = await DatabaseManager.GetOpen();

        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM hwid_bans WHERE hwid = @hwid", conn);
        cmd.Parameters.AddWithValue("@hwid", hwid);

        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return result > 0;
    }
}
