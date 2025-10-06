namespace T.Database;

public class DatabaseLogger
{
    public enum LogLevel
    {
        Info,
        Log,
        Warning,
        Error,
        Exception,
        Critical
    }

    public Action<LogLevel, string>? onLog;

    public void Log(LogLevel level, string message)
        => onLog?.Invoke(level, message);
}
