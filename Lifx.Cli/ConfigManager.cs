using System.Text.Json;

namespace Lifx.Cli;

public class CliConfiguration
{
	public string? ApiToken { get; set; }
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

	public static string GetApiToken(string? overrideToken = null)
	{
		// Priority: override > environment > config file
		if (!string.IsNullOrWhiteSpace(overrideToken))
		{
			return overrideToken;
		}

		var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		if (!string.IsNullOrWhiteSpace(envToken))
		{
			return envToken;
		}

		var config = Load();
		if (!string.IsNullOrWhiteSpace(config.ApiToken))
		{
			return config.ApiToken;
		}

		throw new InvalidOperationException(
			"No API token configured. Set via:" + Environment.NewLine +
			"  1. --token option" + Environment.NewLine +
			"  2. LIFX_API_TOKEN environment variable" + Environment.NewLine +
			"  3. 'dotnet lifx config set-token <token>' command");
	}
}
