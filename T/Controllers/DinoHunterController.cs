using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using T.Db;
using T.External.SimpleJSON;
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

	[HttpPost("userHandler.saveUser")]
	public async Task<IActionResult> SaveUser([FromForm] string? data)
	{
		if (data == null) return Content(missingData);

		await DinoHunterDB.SaveUser(DinoHunterAccount.FromJson(data, false));

		return Ok();
	}

	[HttpPost("userHandler.loadUser")]
	public async Task<IActionResult> LoadUser([FromForm] string? data)
	{
		if (data == null) return Content(missingData);

		JsonNode? jsonData = JsonNode.Parse(data);
		if (jsonData == null) return Ok(unableToParseData);

		JsonNode? userid = jsonData["userId"];
		if (userid == null) return Content(dataMissesNeededValues);

		DinoHunterAccount? user = await DinoHunterDB.LoadUser(userid.ToString());

		if (user == null) return Content(userNotFound);

		return Content(user.ToJson(true).ToJsonString());
	}

	[HttpPost("userHandler.insertLeaderboard")]
	public async Task<IActionResult> InsertLeaderboard([FromForm] string? data)
	{
		if (data == null) return BadRequest();

		await DinoHunterDB.InsertLeaderboard(DinoHunterAccount.FromJson(data, true));

		return Ok();
	}

	[HttpPost("userHandler.listLeaderboard")]
	public async Task<IActionResult> ListLeaderboard([FromForm] string? data)
	{
		if (data == null) return Content(missingData);
		JsonNode? jsonData = JsonNode.Parse(data);
		if (jsonData == null) return Content(unableToParseData);

		JsonNode? userId = jsonData["userId"];
		if (userId == null) return Content(dataMissesNeededValues);

		JsonObject resultIndex = new()
		{
			["code"] = "0",
			["leaderboards"] = UserListToLeaderboard(await DinoHunterDB.ListLeaderboard()),
			["myrank"] = await DinoHunterDB.GetPlaceFor(userId.ToString())
		};

		return Content(resultIndex.ToString());
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
}
