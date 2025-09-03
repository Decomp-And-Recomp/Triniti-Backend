using T.Db;

namespace T;

public class Middleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (await BanDB.IsIpBanned(context))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: Your IP is banned or cant be detected (server issue).");
            return;
        }

        await next(context);
    }
}
