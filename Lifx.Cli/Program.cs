using System.CommandLine;
using Lifx.Cli.Commands;
using Spectre.Console;

namespace Lifx.Cli;

class Program
{
	static async Task<int> Main(string[] args)
	{
		try
		{
			var rootCommand = new RootCommand("LIFX CLI - Control your LIFX smart lights from the command line")
			{
				CreateKeyCommand(),
				LightsCommand.Create(),
				EffectsCommand.Create(),
				ScenesCommand.Create(),
				LanCommand.Create(),
				ConfigCommand.Create()
			};

			// Add global options
			var verboseOption = new Option<bool>(
				aliases: ["--verbose", "-v"],
				description: "Enable verbose output");

			var apiTokenOption = new Option<string?>(
				aliases: ["--token", "-t"],
				description: "LIFX Cloud API token (overrides config)");

			rootCommand.AddGlobalOption(verboseOption);
			rootCommand.AddGlobalOption(apiTokenOption);

			return await rootCommand.InvokeAsync(args);
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}

	private static Command CreateKeyCommand()
	{
		var command = new Command("key", "Manage LIFX Cloud API token (securely stored in Windows Credential Manager)")
		{
			CreateSetKeyCommand(),
			CreateShowKeyCommand(),
			CreateDeleteKeyCommand()
		};

		return command;
	}

	private static Command CreateSetKeyCommand()
	{
		var command = new Command("set", "Store API token securely");

		var tokenArg = new Argument<string>(
			"token",
			description: "API token from https://cloud.lifx.com/settings");

		command.AddArgument(tokenArg);

		command.SetHandler((string token) =>
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				AnsiConsole.MarkupLine("[red]API token cannot be empty[/]");
				return;
			}

			var success = SecureCredentialManager.StoreApiToken(token);

			if (success)
			{
				AnsiConsole.MarkupLine("[green]✓[/] API token stored securely in Windows Credential Manager");
				AnsiConsole.MarkupLine("[dim]You can now use LIFX CLI commands without specifying --token[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]Failed to store API token[/]");
			}
		}, tokenArg);

		return command;
	}

	private static Command CreateShowKeyCommand()
	{
		var command = new Command("show", "Check if API token is configured");

		command.SetHandler(() =>
		{
			var hasToken = SecureCredentialManager.HasStoredToken();

			if (hasToken)
			{
				var token = SecureCredentialManager.GetApiToken();
				var masked = token != null && token.Length > 8
					? $"{token[..4]}...{token[^4..]}"
					: "***";

				AnsiConsole.MarkupLine($"[green]✓[/] API token is configured: [dim]{masked}[/]");
				AnsiConsole.MarkupLine("[dim]Stored in: Windows Credential Manager[/]");
			}
			else
			{
				var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
				if (!string.IsNullOrWhiteSpace(envToken))
				{
					AnsiConsole.MarkupLine("[yellow]⚠[/] No token in Credential Manager, but LIFX_API_TOKEN environment variable is set");
				}
				else
				{
					AnsiConsole.MarkupLine("[red]✗[/] No API token configured");
					AnsiConsole.MarkupLine("[dim]Run: dotnet lifx key set <token>[/]");
				}
			}
		});

		return command;
	}

	private static Command CreateDeleteKeyCommand()
	{
		var command = new Command("delete", "Remove stored API token");

		command.SetHandler(() =>
		{
			if (!SecureCredentialManager.HasStoredToken())
			{
				AnsiConsole.MarkupLine("[yellow]No stored API token found[/]");
				return;
			}

			var success = SecureCredentialManager.DeleteApiToken();

			if (success)
			{
				AnsiConsole.MarkupLine("[green]✓[/] API token removed from Windows Credential Manager");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]Failed to delete API token[/]");
			}
		});

		return command;
	}
}
