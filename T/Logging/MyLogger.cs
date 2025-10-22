
namespace T.Logging;

/// <summary>
/// Logger redirecting everything to <see cref="Logger"/>
/// </summary>
public class MyLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Information) return false;

        return true;
    }

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (exception != null)
        {
            Logger.LogException(exception);
            return;
        }

        LogLevel l = LogLevel.Log;

        switch (logLevel)
        {
            //case Microsoft.Extensions.Logging.LogLevel.Trace: break;
            //case Microsoft.Extensions.Logging.LogLevel.Debug: l = LogLevel.Log; break;
            case Microsoft.Extensions.Logging.LogLevel.Information: l = LogLevel.Info; break;
            case Microsoft.Extensions.Logging.LogLevel.Warning: l = LogLevel.Warning; break;
            case Microsoft.Extensions.Logging.LogLevel.Error: l = LogLevel.Error; break;
            case Microsoft.Extensions.Logging.LogLevel.Critical: l = LogLevel.Critical; break;
        }

        string message = formatter(state, exception);

        Logger.Log(l, message);
    }
}
