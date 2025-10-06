namespace T.Logging;

public enum LogLevel
{
    Info,
    Log,
    Warning,
    Error,
    Exception,
    Critical
}

public static class Logger
{
    static readonly object logLock = new();

    public static void Log(LogLevel level, object? message)
    {
        lock (logLock)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Console.Write("Info      ");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogLevel.Log:
                    Console.Write("Log       ");
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.Write("Warning   ");
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.Write("Error     ");
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Exception:
                    Console.Write("Exception ");
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Critical  ");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.Write(' ');

            Console.BackgroundColor = ConsoleColor.Black;

            Console.Write(' ');

            Console.WriteLine(message!.ToString());

            // discord stuff
            if (string.IsNullOrEmpty(Config.Discord.token)) return;
            if (Config.Discord.loggingChannelId == 0) return;

            string discordMessage = message!.ToString() ?? "null";
            if (discordMessage.Length > 1925) discordMessage = discordMessage[..1925];

            switch (level)
            {
                case LogLevel.Info:
                    External.Discord.Log($"ansi\n\u001b[2;32m\u001b[2;37m\u001b[2;34m{level}\u001b[0m\u001b[2;37m\u001b[0m\u001b[2;32m\u001b[0m: {message}");
                    break;
                case LogLevel.Log:
                    External.Discord.Log($"ansi\n\u001b[2;32m\u001b[2;37m{level}\u001b[0m\u001b[2;32m\u001b[0m: {message}");
                    break;
                case LogLevel.Warning:
                    External.Discord.Log($"ansi\n\u001b[2;31m\u001b[2;33m{level}\u001b[0m\u001b[2;31m\u001b[0m: {message}");
                    break;
                case LogLevel.Error:
                    External.Discord.Log($"ansi\n\u001b[2;31m\u001b[2;33m\u001b[2;31m{level}\u001b[0m\u001b[2;33m\u001b[0m\u001b[2;31m\u001b[0m: {message}");
                    break;
                case LogLevel.Exception:
                    External.Discord.Log($"ansi\n\u001b[2;31m\u001b[2;33m\u001b[2;31m{level}\u001b[0m\u001b[2;33m\u001b[0m\u001b[2;31m\u001b[0m: {message}");
                    break;
                case LogLevel.Critical:
                    External.Discord.Log($"ansi\n\u001b[2;31m\u001b[2;33m\u001b[2;31m{level}\u001b[0m\u001b[2;33m\u001b[0m\u001b[2;31m\u001b[0m: {message}");
                    break;
            }
        }
    }

    public static void Log(object message)
        => Log(LogLevel.Log, message);

    public static void Info(string message)
        => Log(LogLevel.Info, message);

    public static void Warning(string message)
        => Log(LogLevel.Warning, message);

    public static void Error(string message)
        => Log(LogLevel.Error, message);

    public static void LogException(Exception exception)
        => Log(LogLevel.Exception, $"Message: {exception.Message}\nStack Trace: {exception.InnerException}");
}
