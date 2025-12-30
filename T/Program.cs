using Microsoft.AspNetCore.HttpOverrides;

namespace T;

public static class Program
{
    private const string release = "1.2.0";

    public static async Task Main(string[] args)
    {
        Console.WriteLine($"Triniti Backend. Release: {release}");

        Console.WriteLine();

        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.CursorVisible = false;
        // Hide user input
        _ = Task.Run(() => {
            while (true) _ = Console.ReadKey(true);
            });

        SetupCrashCatch();

        await Config.Initialize();

        await DB.Init();

        await External.Discord.Run();

        GetApp(args).Run();
    }

    private static void SetupCrashCatch()
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

    private static WebApplication GetApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls(Config.General.HostUrl);

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
