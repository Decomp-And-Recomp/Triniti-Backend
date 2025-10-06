using MySqlConnector;
using T.Database;

namespace T.MySql;

public class MySqlFilterDatabase : FilterDatabase
{
    protected readonly MySqlDatabase controller;

    public MySqlFilterDatabase(MySqlDatabase controller)
    {
        this.controller = controller;
    }

    public override async Task<string?> Filter(string? value)
    {
        if (value == null) return null;

        using var db = await controller.GetOpen();

        using var cmd = new MySqlCommand(@"
        SELECT COUNT(*)
        FROM filter
        WHERE REGEXP_LIKE(
            @text,
            CONCAT(
                '(^|[^[:alnum:]])',
                REGEXP_REPLACE(badword, '(.)', '($1[^[:alnum:]]*)+'),
                '($|[^[:alnum:]])'
            ),
            'i'  -- case-insensitive
        );", db);

        cmd.Parameters.AddWithValue("@text", value ?? "");

        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0 ? "Moderated" : (value ?? string.Empty);
    }
}
