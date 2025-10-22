using T.Database.Objects;

namespace T.Database;

public abstract class GameConfigDatabase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Ip and Port of the game.</returns>
    public abstract Task<GameConfig?> GetGameConfig(string? game);
}
