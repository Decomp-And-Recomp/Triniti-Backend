using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using T.Db;
using T.External;
using T.Objects;

namespace T.Controllers;

[Route("api/DinoHunter")] 
[ApiController]
public class DinoHunterController : ControllerBase
{
	const string missingData = "{ \"code\": \"1\" }";
	const string unableToParseData = "{ \"code\": \"2\" }";
	const string dataMissesNeededValues = "{ \"code\": \"3\" }";
	const string userNotFound = "{ \"code\": \"4\" }";
	const string encryptionError = "{ \"code\": \"5\" }";

	[HttpPost("userHandler.saveUser")]
	public async Task<IActionResult> SaveUser()
	{
		StreamReader reader = new(Request.Body);
		string data = await reader.ReadToEndAsync();

		if (string.IsNullOrEmpty(data)) return Content(Encrypt(missingData));

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(encryptionError));

        var account = DinoHunterAccount.FromJson(data, false);

		await FilterDB.Filter(account);

		await DinoHunterDB.SaveUser(account, WebUtils.GetIp(Request.HttpContext));

		return Ok();
	}

	[HttpPost("userHandler.loadUser")]
	public async Task<IActionResult> LoadUser()
    {
        StreamReader reader = new(Request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(missingData));

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(encryptionError));

		JsonNode? jsonData = JsonNode.Parse(data);
		if (jsonData == null) return Ok(unableToParseData);

		JsonNode? userid = jsonData["userId"];
		if (userid == null) return Content(Encrypt(dataMissesNeededValues));

		DinoHunterAccount? user = await DinoHunterDB.LoadUser(userid.ToString());

		if (user == null) return Content(userNotFound);

		return Content(Encrypt(user.ToJson(true).ToJsonString()));
	}

	[HttpPost("userHandler.insertLeaderboard")]
	public async Task<IActionResult> InsertLeaderboard()
    {
        StreamReader reader = new(Request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(missingData));

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(encryptionError));

        var account = DinoHunterAccount.FromJson(data, true);

		await FilterDB.Filter(account);

        await DinoHunterDB.InsertLeaderboard(account);

		return Ok();
	}

	[HttpPost("userHandler.listLeaderboard")]
	public async Task<IActionResult> ListLeaderboard()
    {
        StreamReader reader = new(Request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(missingData));

        data = XXTEAUtils.Decrypt(data, Config.encryptionKey);

        if (string.IsNullOrEmpty(data)) return Content(Encrypt(encryptionError));

        JsonNode? jsonData = JsonNode.Parse(data);
        if (jsonData == null) return Content(Encrypt(unableToParseData));

        JsonNode? userId = jsonData["userId"];
        if (userId == null) return Content(Encrypt(dataMissesNeededValues));

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
