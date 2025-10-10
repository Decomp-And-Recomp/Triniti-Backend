namespace T.Database;

public abstract class GameConfigDatabase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Ip and Port of the game.</returns>
    public abstract KeyValuePair<string, int>? GetGameConfig(string? game);
}
