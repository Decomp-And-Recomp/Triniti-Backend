using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using T.AntiCheat;
using T.Helpers;
using T.Extensions;
using T.Database.Objects.DinoHunter;

namespace T.Controllers;

[Route("api/DinoHunter")] 
[ApiController]
public class DinoHunterController : ControllerBase
{
    [HttpPost("userHandler.saveUser")]
    public async Task<IActionResult> SaveUser()
    {
        string data = await Utils.ReadEncryptedBody(Request);

        var entry = DinoHunterHelper.AccountEntryFromJson(data, Utils.GetIp(Request.HttpContext));
        if (entry == null) return BadRequest("Unable to parse the request data.");

        if (await DB.BanDatabase.IsHWIDBanned(entry.userId)) return Forbid("The user is banned.");

        await DinoHunterHelper.Filter(entry);

        await DB.DinoHunterDatabase.SaveUser(entry);

        return Ok();
    }

    [HttpPost("userHandler.loadUser")]
    public async Task<IActionResult> LoadUser()
    {
        string data = await Utils.ReadEncryptedBody(Request);
        if (string.IsNullOrEmpty(data)) return BadRequest();

        string? userid = JsonNode.Parse(data)?["userId"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(userid)) return BadRequest();

        if (await DB.BanDatabase.IsHWIDBanned(userid)) return Forbid("The user is banned.");

        var entry = await DB.DinoHunterDatabase.LoadUser(userid);

        if (entry == null) return BadRequest();

        return Content(Utils.Encrypt(entry.ToJson().ToJsonString()));
    }

    [HttpPost("userHandler.insertLeaderboard")]
    public async Task<IActionResult> InsertLeaderboard()
    {
        string data = await Utils.ReadEncryptedBody(Request);
        if (string.IsNullOrEmpty(data)) return BadRequest();

        var entry = DinoHunterHelper.LeaderboardEntryFromJson(data);
        if (entry == null) return BadRequest();

        if (await DB.BanDatabase.IsHWIDBanned(entry.userId)) return Forbid("The user is banned.");

        await DinoHunterHelper.Filter(entry);

        await DinoHunterAC.ProcessLeaderboard(entry);

        await DB.DinoHunterDatabase.InsertLeaderboard(entry);

        return Ok();
    }

    [HttpPost("userHandler.listLeaderboard")]
    public async Task<IActionResult> ListLeaderboard()
    {
        string data = await Utils.ReadEncryptedBody(Request);

        if (string.IsNullOrEmpty(data)) return BadRequest();

        string? userId = JsonNode.Parse(data)?["userId"]?.GetValue<string>();
        
        if (await DB.BanDatabase.IsHWIDBanned(userId)) return Forbid("The user is banned.");

        JsonObject resultIndex = new()
        {
            ["code"] = "0",
            ["leaderboards"] = EntryListToLeadeboard(
                await DB.DinoHunterDatabase.ListLeaderboard(Config.DinoHunter.MaxLeaderboardReturnAmount)),
            ["myrank"] = await DB.DinoHunterDatabase.GetPlaceFor(userId)
        };

        return Content(Utils.Encrypt(resultIndex.ToJsonString()));
    }

    static JsonArray EntryListToLeadeboard(List<LeaderboardEntry> users)
    {
        JsonArray result = [];

        foreach (var user in users) 
        {
            JsonObject userJson = new()
            {
                ["userId"] = user.userId,
                ["applause"] = user.applause.ToString(),
                ["rankName"] = $"{user.hunterLv}|{user.combatpower}|{user.nickname}"
            };

            result.Add(userJson);
        }

        return result;
    }
}
