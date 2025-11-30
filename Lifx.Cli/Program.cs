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
}
