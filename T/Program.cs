namespace T;

public static class Program
{
    public static async Task Main(string[] args)
	{
		Console.InputEncoding = System.Text.Encoding.UTF8;
		Console.OutputEncoding = System.Text.Encoding.UTF8;

		_ = Debug.StartFileWriting();

		Config.Init();

        await Db.DatabaseManager.Init();

        GetApp(args).Run();
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

		app.MapControllers();

		return app;
	}
}
