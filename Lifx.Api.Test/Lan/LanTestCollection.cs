using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Test collection for LAN protocol tests that can be run independently
/// </summary>
[CollectionDefinition("LAN Tests")]
public class LanTestCollection : ICollectionFixture<LanTestFixture>
{
}

/// <summary>
/// Shared fixture for LAN protocol tests
/// Ensures only one UDP socket is bound to port 56700 at a time
/// </summary>
public class LanTestFixture : IAsyncLifetime
{
	private readonly ILogger _logger;

	public LifxClient? SharedClient { get; private set; }
	public bool IsLanStarted { get; private set; }

	public LanTestFixture()
	{
		_logger = LoggerFactory.Create(builder => { })
			.CreateLogger<LanTestFixture>();
	}

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
