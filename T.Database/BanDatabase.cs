namespace T.Database;

public abstract class BanDatabase
{
    /// <summary>
    /// Checking if IP is banned, if <paramref name="ip"/> is null, returns true;
    /// </summary>
    /// <returns>If IP is banned or not.</returns>
    public abstract Task<bool> IsIpBanned(string? ip);

    /// <summary>
    /// Checking if HWID is banned, if <paramref name="hwid"/> is null, returns true;
    /// </summary>
    /// <returns>If HWID is banned or not.</returns>
    public abstract Task<bool> IsHWIDBanned(string? hwid);

    public abstract Task BanIp(string? ip);
    public abstract Task BanHWID(string? hwid);
}
