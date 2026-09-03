using Microsoft.Extensions.Logging;

internal sealed class XunitLogger(ITestOutputHelper output, string categoryName) : ILogger
{
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	/// <summary>
	/// Performs IsEnabled operation.
	/// </summary>
	public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		var message = formatter(state, exception);
		output.WriteLine($"[{logLevel}] {categoryName}: {message}");

		if (exception is not null)
		{
			output.WriteLine(exception.ToString());
		}
	}
}

internal sealed class XunitLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
	/// <summary>
	/// Performs CreateLogger operation.
	/// </summary>
	public ILogger CreateLogger(string categoryName) => new XunitLogger(output, categoryName);

	/// <summary>
	/// Performs Dispose operation.
	/// </summary>
	public void Dispose()
	{
		// Nothing to release: the loggers this provider hands out only write to the xUnit output
		// helper, which the test framework owns and disposes itself.
	}
}
