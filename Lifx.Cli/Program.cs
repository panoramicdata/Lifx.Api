using Lifx.Cli.Commands;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		try
		{
			var rootCommand = new RootCommand("LIFX CLI - Control your LIFX smart lights from the command line")
			{
				CloudCommand.Create(),
				LanCommand.Create(),
				ConfigCommand.Create(),
				ProductsCommand.Create(),
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
				"Quick Start (Cloud):" + Environment.NewLine +
				"  1. Get API token: https://cloud.lifx.com/settings" + Environment.NewLine +
				"  2. Store token: lifx cloud key set <token>" + Environment.NewLine +
				"  3. Control lights: lifx cloud lights on all" + Environment.NewLine +
				Environment.NewLine +
				"Quick Start (LAN - no token needed):" + Environment.NewLine +
				"  lifx lan discover" + Environment.NewLine +
				Environment.NewLine +
				"Examples:" + Environment.NewLine +
				"  lifx cloud key set <token>         # Store API token (do this first!)" + Environment.NewLine +
				"  lifx cloud lights list             # List all lights" + Environment.NewLine +
				"  lifx cloud lights on all           # Turn on all lights" + Environment.NewLine +
				"  lifx cloud lights color all blue   # Set all lights to blue" + Environment.NewLine +
				"  lifx cloud effects breathe all     # Start breathe effect" + Environment.NewLine +
				"  lifx cloud scenes list             # List available scenes" + Environment.NewLine +
				"  lifx lan discover                  # Discover local devices" + Environment.NewLine +
				Environment.NewLine +
				"Get help for any command:" + Environment.NewLine +
				"  lifx cloud --help" + Environment.NewLine +
				"  lifx lan --help" + Environment.NewLine +
				"  lifx cloud lights --help";

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
