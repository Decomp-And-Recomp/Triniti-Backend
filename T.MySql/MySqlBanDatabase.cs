using MySqlConnector;
using T.Database;

namespace T.MySql;

public class MySqlBanDatabase : BanDatabase
{
    private readonly MySqlDatabase controller;

    public MySqlBanDatabase(MySqlDatabase controller)
    {
        this.controller = controller;
    }

    public override async Task<bool> IsIpBanned(string? ip)
    {
        if (string.IsNullOrEmpty(ip)) return true;

        using var conn = await controller.GetOpen();

        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM ip_bans WHERE ip = @ip", conn);
        cmd.Parameters.AddWithValue("@ip", ip);

        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return result > 0;
    }

    public override async Task<bool> IsHWIDBanned(string? hwid)
    {
        if (string.IsNullOrEmpty(hwid)) return true;

        using var conn = await controller.GetOpen();

        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM hwid_bans WHERE hwid = @hwid", conn);
        cmd.Parameters.AddWithValue("@hwid", hwid);

        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return result > 0;
    }

    public override Task BanIp(string? ip)
    {
        throw new NotImplementedException();
    }

    public override Task BanHWID(string? hwid)
    {
        throw new NotImplementedException();
    }
}
