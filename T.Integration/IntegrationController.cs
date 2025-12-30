namespace T.Integration;

public static class IntegrationController
{
    public static bool running { get; private set; }

    public static async Task Run()
    {
        Logger.Info("Integration server starting...");

        running = true;

        try
        {
            await Server.Run();
        }
        catch (Exception ex)
        {
            Logger.Error("An error accured on the Integration Server, shutting off..");
            Logger.Exception(ex);
        }

        Logger.Warning("Integration server is down.");

        running = false;
    }
}