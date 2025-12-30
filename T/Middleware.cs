namespace T;

public class Middleware(RequestDelegate request)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (await DB.Current.banDatabase.IsIpBanned(Utils.GetIp(context)))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Your IP is banned or cant be detected (server issue).");
                return;
            }

            await request(context);
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                StreamReader reader = new(context.Request.Body);
                string data = await reader.ReadToEndAsync();
                string headerData = "Headers:";
                foreach (var v in context.Request.Headers)
                {
                    headerData += $"\n{v.Key}:";
                    foreach (var v2 in v.Value) headerData += $" '{v2}'";
                }
                Logging.Logger.Exception(ex, $"\nIP: '{Utils.GetIp(context)}'\nPath: '{context.Request.Path}'\n{headerData}\n==== Raw Data Base64\n{data}\n==== End");
            });

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Internal Server Error.");
        }
    }
}
