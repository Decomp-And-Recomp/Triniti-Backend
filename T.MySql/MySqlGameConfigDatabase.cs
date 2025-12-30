using MySqlConnector;
using T.Database;
using T.Database.Objects;

namespace T.MySql;

public class MySqlGameConfigDatabase : GameConfigDatabase
{
    protected readonly MySqlDatabase controller;

    public MySqlGameConfigDatabase(MySqlDatabase controller)
    {
        this.controller = controller;
    }

    public override async Task<GameConfig?> GetGameConfig(string? game)
    {
        if (game == null) return null;

        using var db = await controller.GetOpen();

        using var cmd = new MySqlCommand("SELECT * FROM `game_config` WHERE `game` = @game", db);
        cmd.Parameters.AddWithValue("@game", game);

        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read())
        {
            GameConfig result = new()
            {
                ip = reader["ip"].ToString()!,
                port = Convert.ToInt32(reader["port"]),
                version = reader["version"].ToString()!
            };

            return result;
        }

        return null;
    }
}
