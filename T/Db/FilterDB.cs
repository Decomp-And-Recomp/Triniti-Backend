using MySqlConnector;
using T.Objects;

namespace T.Db;

public static class FilterDB
{
    public static async Task Filter(DinoHunterAccount account)
    {
        string? motto = account.GetMottoFromExts();

        if (motto != null)
        {
            string moderatedMotto = await Filter(motto);
            if (motto != moderatedMotto) account.SetMottoToExts(moderatedMotto);
        }

        if (account.nickname == null) return;

        account.nickname = await Filter(account.nickname);    
    }

    static async Task<string> Filter(string s)
    {
        using var db = await DatabaseManager.GetOpen();

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

        cmd.Parameters.AddWithValue("@text", s ?? "");

        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0 ? "Moderated" : (s ?? string.Empty);
    }
}
