using System.Text.Json;

namespace Lifx.Cli;

/// <summary>
/// Represents the CliConfiguration type.
/// </summary>
public class CliConfiguration
{
	/// <summary>
	/// Gets or sets UseLan.
	/// </summary>
	public bool UseLan { get; set; } = true;
	/// <summary>
	/// Gets or sets DefaultDuration.
	/// </summary>
	public int DefaultDuration { get; set; } = 1000; // milliseconds
	/// <summary>
	/// Gets or sets DefaultSelector.
	/// </summary>
	public string DefaultSelector { get; set; } = "all";
}

/// <summary>
/// Represents the ConfigManager type.
/// </summary>
public static class ConfigManager
{
	private static readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		WriteIndented = true
	};

	private static readonly string ConfigDirectory = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
		".lifx");

	private static readonly string ConfigFile = Path.Combine(ConfigDirectory, "config.json");

	/// <summary>
	/// Performs Load operation.
	/// </summary>
	public static CliConfiguration Load()
	{
		if (!File.Exists(ConfigFile))
		{
			return new CliConfiguration();
		}

		try
		{
			var json = File.ReadAllText(ConfigFile);
			return JsonSerializer.Deserialize<CliConfiguration>(json) ?? new CliConfiguration();
		}
		catch
		{
			return new CliConfiguration();
		}
	}

	/// <summary>
	/// Performs Save operation.
	/// </summary>
	public static void Save(CliConfiguration config)
	{
		Directory.CreateDirectory(ConfigDirectory);
		var json = JsonSerializer.Serialize(config, jsonSerializerOptions);
		File.WriteAllText(ConfigFile, json);
	}

	/// <summary>
	/// Performs TryGetApiToken operation.
	/// </summary>
	public static string? TryGetApiToken() => TryGetApiToken(null);

	/// <summary>
	/// Performs TryGetApiToken operation.
	/// </summary>
	public static string? TryGetApiToken(string? overrideToken)
	{
		// Priority 1: Override token from command line
		if (!string.IsNullOrWhiteSpace(overrideToken))
		{
			return overrideToken;
		}

		// Priority 2: Environment variable
		var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		if (!string.IsNullOrWhiteSpace(envToken))
		{
			return envToken;
		}

		// Priority 3: Windows Credential Store (secure storage)
		var storedToken = SecureCredentialManager.GetApiToken();
		if (!string.IsNullOrWhiteSpace(storedToken))
		{
			return storedToken;
		}

		return null;
	}

	/// <summary>
	/// Performs GetApiToken operation.
	/// </summary>
	public static string GetApiToken() => GetApiToken(null);

	/// <summary>
	/// Performs GetApiToken operation.
	/// </summary>
	public static string GetApiToken(string? overrideToken)
	{
		var token = TryGetApiToken(overrideToken);

		if (string.IsNullOrWhiteSpace(token))
		{
			throw new InvalidOperationException(
				"No LIFX Cloud API token configured." + Environment.NewLine +
				Environment.NewLine +
				"To use LIFX Cloud features, you need an API token:" + Environment.NewLine +
				Environment.NewLine +
				"1. Get your token from https://cloud.lifx.com/settings" + Environment.NewLine +
				"   - Log in to your LIFX account" + Environment.NewLine +
				"   - Click 'Generate New Token'" + Environment.NewLine +
				"   - Copy the generated token" + Environment.NewLine +
				Environment.NewLine +
				"2. Store it securely using:" + Environment.NewLine +
				"   lifx key set <your-token-here>" + Environment.NewLine +
				Environment.NewLine +
				"Alternative options:" + Environment.NewLine +
				"   - Use --token option for one-time use" + Environment.NewLine +
				"   - Set LIFX_API_TOKEN environment variable" + Environment.NewLine +
				Environment.NewLine +
				"Note: LAN features work without an API token.");
		}

		return token;
	}
}
