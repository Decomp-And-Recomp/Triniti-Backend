using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Text.Json.Nodes;
using T.Db;
using T.External;
using T.Objects;

namespace T.Controllers;

[Route("api/DinoHunter")] 
[ApiController]
public class DinoHunterController : ControllerBase
{
	[HttpPost("userHandler.saveUser")]
	public async Task<IActionResult> SaveUser()
	{
		StreamReader reader = new(Request.Body);
		string data = await reader.ReadToEndAsync();

		if (string.IsNullOrEmpty(data)) return BadRequest();

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return BadRequest();

        var account = DinoHunterAccount.FromJson(data, false);

		if (await BanDB.IsHWIDBanned(account.userId)) return BadRequest();

        await FilterDB.Filter(account);

		await DinoHunterDB.SaveUser(account, Utils.GetIp(Request.HttpContext));

		return Ok();
	}

	[HttpPost("userHandler.loadUser")]
	public async Task<IActionResult> LoadUser()
    {
        StreamReader reader = new(Request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(data)) return BadRequest();

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return BadRequest();

        JsonNode? jsonData = JsonNode.Parse(data);
		if (jsonData == null) return BadRequest();

        JsonNode? userid = jsonData["userId"];
		if (userid == null) return BadRequest();
        if (await BanDB.IsHWIDBanned(userid.ToString())) return BadRequest();

        DinoHunterAccount? user = await DinoHunterDB.LoadUser(userid.ToString());

		if (user == null) return BadRequest();

        return Content(Encrypt(user.ToJson(true).ToJsonString()));
	}

	[HttpPost("userHandler.insertLeaderboard")]
	public async Task<IActionResult> InsertLeaderboard()
    {
        StreamReader reader = new(Request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(data)) return BadRequest();

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return BadRequest();

        var account = DinoHunterAccount.FromJson(data, true);
        if (await BanDB.IsHWIDBanned(account.userId)) return BadRequest();

        await FilterDB.Filter(account);

        await DinoHunterDB.InsertLeaderboard(account);

		return Ok();
	}

	[HttpPost("userHandler.listLeaderboard")]
	public async Task<IActionResult> ListLeaderboard()
    {
        StreamReader reader = new(Request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(data)) return BadRequest();

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return BadRequest();

        JsonNode? jsonData = JsonNode.Parse(data);
        if (jsonData == null) return BadRequest();

        JsonNode? userId = jsonData["userId"];
        if (userId == null) return BadRequest();
        if (await BanDB.IsHWIDBanned(userId.ToString())) return BadRequest();

        JsonObject resultIndex = new()
        {
            ["code"] = "0",
            ["leaderboards"] = UserListToLeaderboard(await DinoHunterDB.ListLeaderboard()),
            ["myrank"] = await DinoHunterDB.GetPlaceFor(userId.ToString())
        };

        return Content(Encrypt(resultIndex.ToString()));
	}

	JsonArray UserListToLeaderboard(List<DinoHunterAccount> users)
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

	string Encrypt(string str)
	{
		return XXTEAUtils.Encrypt(str, Config.encryptionKey);
	}
}
