using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace T.Controllers
{
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
    }
}
