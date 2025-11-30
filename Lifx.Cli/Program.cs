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
				ConfigCommand.Create(),
				CreateVersionCommand()
			};

			rootCommand.Description = 
				"LIFX CLI - Control your LIFX smart lights from the command line" + Environment.NewLine +
				Environment.NewLine +
				"Features:" + Environment.NewLine +
				"  - Cloud API: Control lights via internet (requires API token)" + Environment.NewLine +
				"  - LAN Protocol: Control lights on local network (no API token needed)" + Environment.NewLine +
				"  - Secure credential storage via Windows Credential Manager" + Environment.NewLine +
				Environment.NewLine +
				"Quick Start:" + Environment.NewLine +
				"  1. Get API token: https://cloud.lifx.com/settings" + Environment.NewLine +
				"  2. Store token: lifx key set <token>" + Environment.NewLine +
				"  3. Control lights: lifx lights on all" + Environment.NewLine +
				Environment.NewLine +
				"For LAN-only usage (no API token needed):" + Environment.NewLine +
				"  lifx lan discover" + Environment.NewLine +
				Environment.NewLine +
				"Examples:" + Environment.NewLine +
				"  lifx lights list               # List all lights" + Environment.NewLine +
				"  lifx lights on all             # Turn on all lights" + Environment.NewLine +
				"  lifx lights color all blue     # Set all lights to blue" + Environment.NewLine +
				"  lifx effects breathe all       # Start breathe effect" + Environment.NewLine +
				"  lifx scenes list               # List available scenes" + Environment.NewLine +
				Environment.NewLine +
				"Get help for any command:" + Environment.NewLine +
				"  lifx lights --help" + Environment.NewLine +
				"  lifx effects breathe --help";

			// Add global options
			var verboseOption = new Option<bool>(
				aliases: ["--verbose", "-v"],
				description: "Enable verbose output with detailed information");

			var apiTokenOption = new Option<string?>(
				aliases: ["--token", "-t"],
				description: "LIFX Cloud API token (overrides stored credential)");

			rootCommand.AddGlobalOption(verboseOption);
			rootCommand.AddGlobalOption(apiTokenOption);

			return await rootCommand.InvokeAsync(args);
		}
		catch (InvalidOperationException ex) when (ex.Message.Contains("No LIFX Cloud API token"))
		{
			// User-friendly error for missing API token
			AnsiConsole.MarkupLine("[yellow]? API Token Required[/]");
			AnsiConsole.WriteLine();
			AnsiConsole.WriteLine(ex.Message);
			return 1;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			if (args.Contains("--verbose") || args.Contains("-v"))
			{
				AnsiConsole.WriteException(ex);
			}
			return 1;
		}
	}

	private static Command CreateKeyCommand()
	{
		var command = new Command("key", "Manage LIFX Cloud API token")
		{
			CreateSetKeyCommand(),
			CreateShowKeyCommand(),
			CreateDeleteKeyCommand()
		};

		command.Description = 
			"Manage LIFX Cloud API token (securely stored in Windows Credential Manager)" + Environment.NewLine +
			Environment.NewLine +
			"The API token is required for Cloud API features but NOT for LAN features." + Environment.NewLine +
			Environment.NewLine +
			"Get your API token:" + Environment.NewLine +
			"  1. Visit https://cloud.lifx.com/settings" + Environment.NewLine +
			"  2. Log in to your LIFX account" + Environment.NewLine +
			"  3. Click 'Generate New Token'" + Environment.NewLine +
			"  4. Copy the token and store it using: lifx key set <token>";

		return command;
	}

	private static Command CreateSetKeyCommand()
	{
		var command = new Command("set", "Store API token securely in Windows Credential Manager");

		var tokenArg = new Argument<string>(
			"token",
			description: "API token from https://cloud.lifx.com/settings");

		command.AddArgument(tokenArg);

		command.SetHandler((string token) =>
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				AnsiConsole.MarkupLine("[red]? API token cannot be empty[/]");
				return;
			}

			var success = SecureCredentialManager.StoreApiToken(token);

			if (success)
			{
				AnsiConsole.MarkupLine("[green]? API token stored securely[/]");
				AnsiConsole.MarkupLine("[dim]Location: Windows Credential Manager[/]");
				AnsiConsole.WriteLine();
				AnsiConsole.MarkupLine("You can now use LIFX Cloud commands:");
				AnsiConsole.MarkupLine("  [cyan]lifx lights list[/]");
				AnsiConsole.MarkupLine("  [cyan]lifx lights on all[/]");
				AnsiConsole.MarkupLine("  [cyan]lifx effects breathe all[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]? Failed to store API token[/]");
				AnsiConsole.MarkupLine("[dim]Check Windows Credential Manager permissions[/]");
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

				AnsiConsole.MarkupLine($"[green]? API token is configured[/]");
				AnsiConsole.MarkupLine($"[dim]Token: {masked}[/]");
				AnsiConsole.MarkupLine($"[dim]Storage: Windows Credential Manager[/]");
			}
			else
			{
				var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
				if (!string.IsNullOrWhiteSpace(envToken))
				{
					AnsiConsole.MarkupLine("[yellow]? API token from environment variable[/]");
					AnsiConsole.MarkupLine("[dim]LIFX_API_TOKEN is set[/]");
					AnsiConsole.MarkupLine("[dim]Tip: Use 'lifx key set' for secure storage[/]");
				}
				else
				{
					AnsiConsole.MarkupLine("[red]? No API token configured[/]");
					AnsiConsole.WriteLine();
					AnsiConsole.MarkupLine("To get an API token:");
					AnsiConsole.MarkupLine("  1. Visit [link]https://cloud.lifx.com/settings[/]");
					AnsiConsole.MarkupLine("  2. Log in and click 'Generate New Token'");
					AnsiConsole.MarkupLine("  3. Run: [cyan]lifx key set <token>[/]");
					AnsiConsole.WriteLine();
					AnsiConsole.MarkupLine("[dim]Note: LAN features work without an API token[/]");
				}
			}
		});

		return command;
	}

	private static Command CreateDeleteKeyCommand()
	{
		var command = new Command("delete", "Remove stored API token from Windows Credential Manager");

		command.SetHandler(() =>
		{
			if (!SecureCredentialManager.HasStoredToken())
			{
				AnsiConsole.MarkupLine("[yellow]? No stored API token found[/]");
				return;
			}

			var success = SecureCredentialManager.DeleteApiToken();

			if (success)
			{
				AnsiConsole.MarkupLine("[green]? API token removed[/]");
				AnsiConsole.MarkupLine("[dim]Removed from Windows Credential Manager[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]? Failed to delete API token[/]");
			}
		});

		return command;
	}

	private static Command CreateVersionCommand()
	{
		var command = new Command("version", "Show version information");

		command.SetHandler(() =>
		{
			var version = typeof(Program).Assembly.GetName().Version;
			AnsiConsole.MarkupLine($"[cyan]LIFX CLI[/] version [green]{version}[/]");
			AnsiConsole.MarkupLine("[dim]https://github.com/panoramicdata/Lifx.Api[/]");
		});

		return command;
	}
}
