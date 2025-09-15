using System.Text;
using T.Db;
using T.Objects;

namespace T.AntiCheat;

public static class DinoHunterAC
{
    public static async Task ProcessLeaderboard(DinoHunterAccount account)
    {
        if (!Config.enableAntiCheat) return;

        try
        {
            bool send = false;

            StringBuilder builder = new();
            builder.AppendLine("[Dino Hunter] Activity report");
            builder.AppendLine($"User ID: {account.userId}");
            builder.AppendLine($"Name: {account.nickname}");
            builder.AppendLine("===============");
            builder.AppendLine("Detected:");

            void Add(string msg)
            {
                builder.AppendLine(msg);
                send = true;
            }

            if (account.gold > 600000) Add($"Gold Amount: {account.gold}");
            if (account.crystal > 1250) Add($"Crystal Amount: {account.crystal}");
            if (account.hunterLv > 250) Add($"Hunter Level: {account.hunterLv}");

            var oldEntry = await DinoHunterDB.GetFromLeaderboard(account.userId!);

            if (oldEntry != null)
            {
                if (oldEntry.userId == account.userId)
                {
                    int? goldChange = account.gold - oldEntry.gold;
                    int? crystalChange = account.crystal - oldEntry.crystal;
                    int? hunterLvChange = account.hunterLv - oldEntry.hunterLv;

                    if (goldChange > 5000) Add($"Gold {oldEntry.gold} > {account.gold}");
                    if (crystalChange > 100) Add($"Crystal {oldEntry.crystal} > {account.crystal}");

                    if (hunterLvChange > 20) Add($"Hunter Level {oldEntry.hunterLv} > {account.hunterLv}");
                    else if (hunterLvChange < -20) Add($"Hunter Level {oldEntry.hunterLv} > {account.hunterLv}");

                    if (oldEntry.nickname != account.nickname) Add($"Nickname: {oldEntry.nickname} > {account.nickname}");
                }
                else Add($"=====\nThis fucker sent me User ID for the wrong person\nGot: {oldEntry.userId}");
            }
            else Add($"No Entry (new account).");

            if (send) await Webhook.Send(builder.ToString());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
