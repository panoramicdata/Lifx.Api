using System.Text.Json;

namespace Lifx.Cli;

public class CliConfiguration
{
	public bool UseLan { get; set; } = true;
	public int DefaultDuration { get; set; } = 1000; // milliseconds
	public string DefaultSelector { get; set; } = "all";
}

public static class ConfigManager
{
	private static readonly string ConfigDirectory = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
		".lifx");

	private static readonly string ConfigFile = Path.Combine(ConfigDirectory, "config.json");

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

	public static void Save(CliConfiguration config)
	{
		Directory.CreateDirectory(ConfigDirectory);
		var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
		{
			WriteIndented = true
		});
		File.WriteAllText(ConfigFile, json);
	}

	/// <summary>
	/// Gets the API token with priority: override > environment > Windows Credential Store
	/// </summary>
	public static string GetApiToken(string? overrideToken = null)
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

		throw new InvalidOperationException(
			"No API token configured. Set via:" + Environment.NewLine +
			"  1. dotnet lifx key <token>          - Store securely in Windows Credential Manager (recommended)" + Environment.NewLine +
			"  2. --token option                   - Override for single command" + Environment.NewLine +
			"  3. LIFX_API_TOKEN environment var   - Set in environment");
	}
}
