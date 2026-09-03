using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test;

/// <summary>
/// Represents the Test type.
/// </summary>
public abstract class Test(ITestOutputHelper testOutputHelper)
{
	/// <summary>
	/// Gets the logger used by test classes.
	/// </summary>
	protected ILogger Logger { get; } = LoggerFactory.Create(builder => builder
			.AddProvider(new XunitLoggerProvider(testOutputHelper)))
			.CreateLogger<Test>();

	/// <summary>
	/// Gets the configured LIFX client for integration tests.
	/// </summary>
	protected LifxClient Client { get; } = CreateClient(GetTestConfiguration());

	/// <summary>
	/// Gets the cancellation token for the current test context.
	/// </summary>
	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

	/// <summary>
	/// Gets test configuration loaded from configuration sources.
	/// </summary>
	protected static TestConfiguration Configuration { get; } = GetTestConfiguration();

	private static LifxClient CreateClient(TestConfiguration config)
	{
		var logger = LoggerFactory.Create(builder => builder
			.AddProvider(new XunitLoggerProvider(null!)))
			.CreateLogger<LifxClient>();

		return new LifxClient(new LifxClientOptions
		{
			ApiToken = config.AppToken,
			Logger = logger,
			IsLanEnabled = config.EnableLanTests
		});
	}

	private static TestConfiguration GetTestConfiguration()
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("../../../appsettings.json", optional: true)
			.AddUserSecrets<Test>()
			.Build();

		var appToken = configuration["AppToken"];

		if (string.IsNullOrEmpty(appToken))
		{
			throw new InvalidOperationException(
				"AppToken not found. Please either:\n" +
				"1. Set it in User Secrets using: dotnet user-secrets set \"AppToken\" \"your-token-here\"\n" +
				"2. Copy appsettings.example.json to appsettings.json and set the AppToken value\n" +
				"Get your token from https://cloud.lifx.com/settings");
		}

		return new TestConfiguration
		{
			AppToken = appToken,
			TestLightId = configuration["TestLightId"],
			TestLightLabel = configuration["TestLightLabel"],
			TestGroupId = configuration["TestGroupId"],
			TestGroupLabel = configuration["TestGroupLabel"],
			TestLocationId = configuration["TestLocationId"],
			TestLocationLabel = configuration["TestLocationLabel"],
			EnableLanTests = bool.TryParse(configuration["EnableLanTests"], out var enableLan) && enableLan,
			LanTestTimeout = int.TryParse(configuration["LanTestTimeout"], out var timeout) ? timeout : 10000
		};
	}

	/// <summary>
	/// Gets the first available light or throws if none found
	/// </summary>
	protected async Task<Light> GetTestLightAsync()
	{
		// Try to get specific test light if configured
		if (!string.IsNullOrEmpty(Configuration.TestLightId))
		{
			var lights = await Client.Lights.ListAsync(new Selector.LightId(Configuration.TestLightId!), CancellationToken);
			if (lights.Count > 0)
			{
				return lights[0];
			}
		}

		if (!string.IsNullOrEmpty(Configuration.TestLightLabel))
		{
			var lights = await Client.Lights.ListAsync(new Selector.LightLabel(Configuration.TestLightLabel!), CancellationToken);
			if (lights.Count > 0)
			{
				return lights[0];
			}
		}

		// Fall back to first available light
		var allLights = await Client.Lights.ListAsync(Selector.All, CancellationToken);
		if (allLights.Count == 0)
		{
			throw new InvalidOperationException("No lights found. Please ensure you have LIFX lights configured.");
		}

		return allLights[0];
	}

	/// <summary>
	/// Gets the test group or throws if none found
	/// </summary>
	protected async Task<Group> GetTestGroupAsync()
	{
		var groups = await Client.Lights.ListGroupsAsync(Selector.All, CancellationToken);
		if (groups.Count == 0)
		{
			throw new InvalidOperationException("No groups found.");
		}

		// Try to get specific test group if configured
		if (!string.IsNullOrEmpty(Configuration.TestGroupId))
		{
			var group = groups.FirstOrDefault(g => g.Id == Configuration.TestGroupId);
			if (group is not null)
			{
				return group;
			}
		}

		if (!string.IsNullOrEmpty(Configuration.TestGroupLabel))
		{
			var group = groups.FirstOrDefault(g => g.Label == Configuration.TestGroupLabel);
			if (group is not null)
			{
				return group;
			}
		}

		return groups[0];
	}
}

/// <summary>
/// Represents the TestConfiguration type.
/// </summary>
public class TestConfiguration
{
	/// <summary>
	/// Gets or sets AppToken.
	/// </summary>
	public string AppToken { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets TestLightId.
	/// </summary>
	public string? TestLightId { get; set; }
	/// <summary>
	/// Gets or sets TestLightLabel.
	/// </summary>
	public string? TestLightLabel { get; set; }
	/// <summary>
	/// Gets or sets TestGroupId.
	/// </summary>
	public string? TestGroupId { get; set; }
	/// <summary>
	/// Gets or sets TestGroupLabel.
	/// </summary>
	public string? TestGroupLabel { get; set; }
	/// <summary>
	/// Gets or sets TestLocationId.
	/// </summary>
	public string? TestLocationId { get; set; }
	/// <summary>
	/// Gets or sets TestLocationLabel.
	/// </summary>
	public string? TestLocationLabel { get; set; }
	/// <summary>
	/// Gets or sets EnableLanTests.
	/// </summary>
	public bool EnableLanTests { get; set; }
	/// <summary>
	/// Gets or sets LanTestTimeout.
	/// </summary>
	public int LanTestTimeout { get; set; } = 10000;
}
