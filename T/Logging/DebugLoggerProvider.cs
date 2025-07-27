namespace T.Logging;

public class DebugLoggerProvider : ILoggerProvider
{
	public ILogger CreateLogger(string categoryName)
	{
		return new DebugLogger();
	}

	public void Dispose()
	{
	}
}
