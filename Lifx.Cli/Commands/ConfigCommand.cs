using System.CommandLine;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class ConfigCommand
{
	public static Command Create()
	{
		var command = new Command("config", "Manage CLI configuration (non-sensitive settings)")
		{
			CreateShowCommand(),
			CreateResetCommand()
		};

		return command;
	}

	private static Command CreateShowCommand()
	{
		var command = new Command("show", "Show current configuration");

		command.SetHandler(() =>
		{
			var config = ConfigManager.Load();

			var table = new Table();
			table.Border = TableBorder.Rounded;
			table.AddColumn("Setting");
			table.AddColumn("Value");

			// API Token status
			var hasSecureToken = SecureCredentialManager.HasStoredToken();
			var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
			
			string tokenStatus;
			if (hasSecureToken)
			{
				tokenStatus = "[green]Stored securely in Credential Manager[/]";
			}
			else if (!string.IsNullOrWhiteSpace(envToken))
			{
				tokenStatus = "[yellow]From environment variable[/]";
			}
			else
			{
				tokenStatus = "[red]Not configured[/]";
			}

			table.AddRow("API Token", tokenStatus);
			table.AddRow("Use LAN", config.UseLan.ToString());
			table.AddRow("Default Duration", $"{config.DefaultDuration}ms");
			table.AddRow("Default Selector", config.DefaultSelector);

			AnsiConsole.Write(table);

			// Show config file location
			var configFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				".lifx",
				"config.json");

			AnsiConsole.MarkupLine($"\n[dim]Config file: {configFile}[/]");
			
			if (hasSecureToken)
			{
				AnsiConsole.MarkupLine("[dim]API token stored securely in Windows Credential Manager[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[dim]To set API token: dotnet lifx key set <token>[/]");
			}
		});

		return command;
	}

	private static Command CreateResetCommand()
	{
		var command = new Command("reset", "Reset configuration to defaults (does not delete API token)");

		command.SetHandler(() =>
		{
			var configFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				".lifx",
				"config.json");

			if (File.Exists(configFile))
			{
				File.Delete(configFile);
				AnsiConsole.MarkupLine("[green]?[/] Configuration reset to defaults");
			}
			else
			{
				AnsiConsole.MarkupLine("[yellow]Configuration already at defaults[/]");
			}

			AnsiConsole.MarkupLine("[dim]Note: API token in Credential Manager was not affected[/]");
			AnsiConsole.MarkupLine("[dim]To delete API token: dotnet lifx key delete[/]");
		});

		return command;
	}
}
