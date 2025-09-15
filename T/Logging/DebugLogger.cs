
namespace T.Logging;

/// <summary>
/// Logger redirecting everything to <see cref="Debug"/>
/// </summary>
public class DebugLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string message = formatter(state, exception);

        if (exception != null)
        {
            Debug.LogException(exception);
            return;
        }

        Debug.Log(message);
    }
}
