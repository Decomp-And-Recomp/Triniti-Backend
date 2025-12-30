using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using T.External;
using T.Logging;

namespace T.Controllers;

[Route("api/misc")]
[ApiController]
public class MiscController : ControllerBase
{
    [HttpGet("getTime")]
    public IActionResult GetTime()
    {
        DateTime time = DateTime.UtcNow;

        JsonObject index = new()
        {
            ["year"] = time.Year,
            ["month"] = time.Month,
            ["day"] = time.Day,
            ["hour"] = time.Hour,
            ["minute"] = time.Minute,
            ["second"] = time.Second
        };

        return Content(index.ToJsonString());
    }

    [HttpGet("getTimeMs")]
    public IActionResult GetTimeMs()
    {
        return Content(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
    }

    [HttpPost("getGameConfig")]
    public async Task<IActionResult> GetGame()
    {
        string body = await Utils.ReadEncryptedBody(Request);

        var config = await DB.GameConfigDatabase.GetGameConfig(body);

        if (config == null) return BadRequest();

        return Ok(XXTEAUtils.Encrypt($"{config.ip}|{config.port}|{config.version}", Config.General.EncryptionKey));
    }
}
