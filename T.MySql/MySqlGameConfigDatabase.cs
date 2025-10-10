using MySqlConnector;
using T.Database;

namespace T.MySql;

public class MySqlGameConfigDatabase : GameConfigDatabase
{
    protected readonly MySqlDatabase controller;

    public MySqlGameConfigDatabase(MySqlDatabase controller)
    {
        this.controller = controller;
    }

    public override KeyValuePair<string, int>? GetGameConfig(string? game)
    {
        throw new NotImplementedException();
    }
}
