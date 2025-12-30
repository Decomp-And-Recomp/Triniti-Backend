using System.Text.Json.Nodes;
using T.Database.Objects.DinoHunter;
using T.Extensions;

namespace T.Helpers;

public static class DinoHunterHelper
{
    public static AccountEntry? AccountEntryFromJson(string json, string? ip)
    {
        var index = JsonNode.Parse(json);
        if (index == null) return null;

        var result = new AccountEntry
        {
            userId = index["userId"]?.GetValue<string>(),
            nickname = index["nickName"]?.GetValue<string>() ?? "empty",
            title = index["title"]?.GetValue<int>() ?? 0,
            exts = index["exts"]?.GetValue<string>(),
            ip = ip
        };

        if (string.IsNullOrWhiteSpace(result.userId)) return null;

        return result;
    }

    public static LeaderboardEntry? LeaderboardEntryFromJson(string json)
    {
        var index = JsonNode.Parse(json);
        if (index == null) return null;

        var result = new LeaderboardEntry
        {
            userId = index["userId"]?.GetValue<string>(),
            nickname = index["nickName"]?.GetValue<string>() ?? "empty",
            combatpower = index["combatpower"]?.GetValue<int>(),
            exp = index["exp"]?.GetValue<int>(),
            hunterLv = index["hunterLv"]?.GetValue<int>(),
            crystal = index["crystal"]?.GetValue<int>(),
            gold = index["gold"]?.GetValue<int>(),
            applause = index["applause"]?.GetValue<int>(),
        };

        if (string.IsNullOrWhiteSpace(result.userId)) return null;

        return result;
    }

    public static async Task Filter(LeaderboardEntry entry)
    {
        entry.nickname = ProfanityHelper.Filter(entry.nickname);
    }

    public static async Task Filter(AccountEntry entry)
    {
        entry.nickname = ProfanityHelper.Filter(entry.nickname);

        string? motto = entry.GetMottoFromExts();

        string filteredMotto = ProfanityHelper.Filter(motto);

        if (motto != filteredMotto) entry.SetMottoToExts(filteredMotto);
    }
}
