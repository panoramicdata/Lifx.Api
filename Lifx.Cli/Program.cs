using Lifx.Cli.Commands;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli;

/// <summary>
/// Represents the Program type.
/// </summary>
public static class Program
{
	/// <summary>
	/// Performs Main operation.
	/// </summary>
	public static int Main(string[] args)
	{
		try
		{
			return CreateRootCommand().Parse(args).Invoke();
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

	private static RootCommand CreateRootCommand()
	{
		var rootCommand = new RootCommand("LIFX CLI - Control your LIFX smart lights from the command line")
		{
			CloudCommand.Create(),
			LanCommand.Create(),
			ConfigCommand.Create(),
			ProductsCommand.Create(),
			CreateVersionCommand()
		};

		rootCommand.Description = RootDescription;

		// Add global options
		rootCommand.Options.Add(GlobalOptions.Verbose);
		rootCommand.Options.Add(GlobalOptions.Token);

		return rootCommand;
	}

	private static string RootDescription => string.Join(Environment.NewLine,
	[
		"LIFX CLI - Control your LIFX smart lights from the command line",
		"",
		"Features:",
		"  - Cloud API: Control lights via internet (requires API token)",
		"  - LAN Protocol: Control lights on local network (no API token needed)",
		"  - Secure credential storage via Windows Credential Manager",
		"",
		"Quick Start (Cloud):",
		"  1. Get API token: https://cloud.lifx.com/settings",
		"  2. Store token: lifx cloud key set <token>",
		"  3. Control lights: lifx cloud lights on all",
		"",
		"Quick Start (LAN - no token needed):",
		"  lifx lan discover",
		"",
		"Examples:",
		"  lifx cloud key set <token>         # Store API token (do this first!)",
		"  lifx cloud lights list             # List all lights",
		"  lifx cloud lights on all           # Turn on all lights",
		"  lifx cloud lights color all blue   # Set all lights to blue",
		"  lifx cloud effects breathe all     # Start breathe effect",
		"  lifx cloud scenes list             # List available scenes",
		"  lifx lan discover                  # Discover local devices",
		"",
		"Get help for any command:",
		"  lifx cloud --help",
		"  lifx lan --help",
		"  lifx cloud lights --help"
	]);

	private static Command CreateVersionCommand()
	{
		var command = new Command("version", "Show version information");

		command.SetAction(parseResult =>
		{
			var version = typeof(Program).Assembly.GetName().Version;
			AnsiConsole.MarkupLine($"[cyan]LIFX CLI[/] version [green]{version}[/]");
			AnsiConsole.MarkupLine("[dim]https://github.com/panoramicdata/Lifx.Api[/]");
		});

		return command;
	}
}
