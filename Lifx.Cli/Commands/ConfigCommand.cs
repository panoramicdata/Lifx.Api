using System.CommandLine;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class ConfigCommand
{
	public static Command Create()
	{
		var command = new Command("config", "Manage CLI configuration")
		{
			CreateSetTokenCommand(),
			CreateShowCommand(),
			CreateClearCommand()
		};

		return command;
	}

	private static Command CreateSetTokenCommand()
	{
		var command = new Command("set-token", "Set LIFX Cloud API token");

		var tokenArg = new Argument<string>("token", description: "API token from https://cloud.lifx.com/settings");

		command.AddArgument(tokenArg);

		command.SetHandler((string token) =>
		{
			var config = ConfigManager.Load();
			config.ApiToken = token;
			ConfigManager.Save(config);

			AnsiConsole.MarkupLine("[green]?[/] API token saved");
			AnsiConsole.MarkupLine($"[dim]Config location: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".lifx", "config.json")}[/]");
		}, tokenArg);

		return command;
	}

	private static Command CreateShowCommand()
	{
		var command = new Command("show", "Show current configuration");

		command.SetHandler(() =>
		{
			var config = ConfigManager.Load();

			var table = new Table();
			table.AddColumn("Setting");
			table.AddColumn("Value");

			table.AddRow("API Token", config.ApiToken != null ? "[dim]***configured***[/]" : "[red]not set[/]");
			table.AddRow("Use LAN", config.UseLan.ToString());
			table.AddRow("Default Duration", $"{config.DefaultDuration}ms");
			table.AddRow("Default Selector", config.DefaultSelector);

			var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
			if (!string.IsNullOrWhiteSpace(envToken))
			{
				table.AddRow("Environment Token", "[dim]***set***[/]");
			}

			AnsiConsole.Write(table);
			AnsiConsole.MarkupLine($"\n[dim]Config location: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".lifx", "config.json")}[/]");
		});

		return command;
	}

	private static Command CreateClearCommand()
	{
		var command = new Command("clear", "Clear configuration");

		command.SetHandler(() =>
		{
			var configFile = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				".lifx",
				"config.json");

			if (File.Exists(configFile))
			{
				File.Delete(configFile);
				AnsiConsole.MarkupLine("[green]?[/] Configuration cleared");
			}
			else
			{
				AnsiConsole.MarkupLine("[yellow]No configuration file found[/]");
			}
		});

		return command;
	}
}
