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
            if (motto != moderatedMotto) account.SetMottoToExts(motto);
        }

        if (account.nickname == null) return;

        account.nickname = await Filter(account.nickname);    
    }

    static async Task<string> Filter(string s)
    {
        using var db = await DatabaseManager.GetOpen();

        using var cmd = new MySqlCommand(@"SELECT COUNT(*) FROM filter
            WHERE @text LIKE CONCAT('%', badword, '%')", db);

        cmd.Parameters.AddWithValue("@text", s);

        if (Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0) return "Moderated";

        return s;
    }
}
