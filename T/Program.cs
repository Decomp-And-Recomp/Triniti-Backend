using Microsoft.AspNetCore.HttpOverrides;

namespace T;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.CursorVisible = false;

        //_ = Debug.StartFileWriting();

        SetupCrashCatch();

        Config.Init();

        await Db.DatabaseManager.Init();

        GetApp(args).Run();
    }

    static void SetupCrashCatch()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            try
            {
                var ex = e.ExceptionObject as Exception;

                File.AppendAllText("fatal.txt", $"[{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}]: {ex}{Environment.NewLine}");
            }
            catch
            {
                // ignored
            }
        };
    }

    static WebApplication GetApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls(Config.hostUrl);
        //builder.WebHost.UseUrls("http://127.0.0.4:85/");

        builder.Services.AddControllers();

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new Logging.DebugLoggerProvider());

        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseMiddleware<Middleware>();

        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
