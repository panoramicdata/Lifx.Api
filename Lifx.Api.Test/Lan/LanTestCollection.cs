using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Test collection for LAN protocol tests that can be run independently
/// </summary>
/// <summary>
/// Represents the LanTestCollection type.
/// </summary>
[CollectionDefinition("LAN Tests")]
public class LanTestCollection : ICollectionFixture<LanTestFixture>
{
}

/// <summary>
/// Represents the LanTestFixture type.
/// </summary>
public class LanTestFixture : IAsyncLifetime
{
	private readonly ILogger _logger;

	/// <summary>
	/// Gets or sets SharedClient.
	/// </summary>
	public LifxClient? SharedClient { get; private set; }
	/// <summary>
	/// Gets or sets IsLanStarted.
	/// </summary>
	public bool IsLanStarted { get; private set; }

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public LanTestFixture()
	{
		_logger = LoggerFactory.Create(builder => { })
			.CreateLogger<LanTestFixture>();
	}

	/// <summary>
	/// Performs InitializeAsync operation.
	/// </summary>
	public async ValueTask InitializeAsync()
	{
		// Create a single shared LAN client for all tests in this collection
		SharedClient = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = true
		});

		// Start the LAN client once
		try
		{
			SharedClient.StartLan(CancellationToken.None);
			IsLanStarted = true;
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to start LAN client in fixture");
			IsLanStarted = false;
		}

		await Task.CompletedTask;
	}

	/// <summary>
	/// Performs DisposeAsync operation.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (SharedClient is not null)
		{
			SharedClient.Dispose();
			await Task.Delay(100); // Give time for socket cleanup
		}

		GC.SuppressFinalize(this);
	}
}
