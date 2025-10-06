using T.Database.Objects.DinoHunter;

namespace T.Database;

public abstract class DinoHunterDatabase
{
    /// <summary>
    /// Saves user in the database.
    /// </summary>
    public abstract Task SaveUser(AccountEntry account);

    /// <summary>
    /// Loads user from the database. <br/>
    /// if <paramref name="userId"/> is null, returns null. <br/>
    /// if user doesnt exist, returns null;
    /// </summary>
    public abstract Task<AccountEntry?> LoadUser(string? userId);

    /// <summary>
    /// Inserts an entry in the leaderboard.
    /// </summary>
    public abstract Task InsertLeaderboard(LeaderboardEntry entry);

    /// <summary>
    /// Returns a top list with specified <paramref name="amount"/> of entries sorted by the entry's stats.
    /// </summary>
    public abstract Task<List<LeaderboardEntry>> ListLeaderboard(int amount);

    /// <summary>
    /// Returns an leaderboard entry with provided <paramref name="userId"/> <br/>
    /// If doesnt exist, returns null.
    /// </summary>
    public abstract Task<LeaderboardEntry?> FromLeaderboard(string? userId);

    /// <summary>
    /// Returns the place for a <paramref name="userId"/> in the leaderboard. <br/>
    /// If <paramref name="userId"/> is null, returns -1. <br/>
    /// If entry doesnt exist, returns null.
    /// </summary>
    public abstract Task<int> GetPlaceFor(string? userId);
}
