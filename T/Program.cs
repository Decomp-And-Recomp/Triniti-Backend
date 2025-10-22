using Microsoft.AspNetCore.HttpOverrides;

namespace T;

public static class Program
{
    const string release = "1.1.0";

    public static async Task Main(string[] args)
    {
        Console.WriteLine($"Triniti Backend. Release: {release}");

        Console.WriteLine();

        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.CursorVisible = false;

        SetupCrashCatch();

        await Config.Initialize();

        await DB.Init();

        await External.Discord.Run();

        GetApp(args).Run();
    }

    static void SetupCrashCatch()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            try
            {
                var ex = e.ExceptionObject as Exception;

                File.AppendAllText("fatal.txt", $"[{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}]: {ex}\n\n");
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
        builder.WebHost.UseUrls(Config.General.hostUrl);

        builder.Services.AddControllers();

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new Logging.MyLoggerProvider());

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
