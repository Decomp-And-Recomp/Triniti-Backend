namespace T.Integration;

public enum LogLevel
{
    Info,
    Log,
    Warning,
    Error,
    Exception,
    Critical
}

internal class Logger
{
    public static void Log(LogLevel level, object? message)
    {

    }

    public static void Log(object? message)
    => Log(LogLevel.Log, message);

    public static void Info(object? message)
        => Log(LogLevel.Info, message);

    public static void Warning(object? message)
        => Log(LogLevel.Warning, message);

    public static void Error(object? message)
        => Log(LogLevel.Error, message);

    public static void Exception(Exception exception)
        => Log(LogLevel.Exception, $"Message: {exception.Message}\nStack Trace: {exception.StackTrace}");
}
