using System.CommandLine;
using Lifx.Api;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Requests;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class LightsCommand
{
	public static Command Create()
	{
		var command = new Command("lights", "Control LIFX lights")
		{
			CreateListCommand(),
			CreateOnCommand(),
			CreateOffCommand(),
			CreateToggleCommand(),
			CreateColorCommand(),
			CreateBrightnessCommand()
		};

		return command;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List all lights, groups, or locations");

		var typeOption = new Option<string>(
			aliases: ["--type", "-t"],
			getDefaultValue: () => "lights",
			description: "Type to list (lights, groups, locations)");

		var selectorOption = new Option<string>(
			aliases: ["--selector", "-s"],
			getDefaultValue: () => "all",
			description: "Selector for filtering");

		command.AddOption(typeOption);
		command.AddOption(selectorOption);

		command.SetHandler(async (string? token, bool verbose, string type, string selector) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var selectorObj = SelectorParser.ParseSelector(selector);

			switch (type.ToLowerInvariant())
			{
				case "lights":
					await ListLights(client, selectorObj, verbose);
					break;
				case "groups":
					await ListGroups(client, selectorObj, verbose);
					break;
				case "locations":
					await ListLocations(client, selectorObj, verbose);
					break;
				default:
					AnsiConsole.MarkupLine($"[red]Unknown type: {type}[/]");
					break;
			}
		}, 
		new TokenBinder(),
		new VerboseBinder(),
		typeOption,
		selectorOption);

		return command;
	}

	private static async Task ListLights(LifxClient client, Selector selector, bool verbose)
	{
		var lights = await client.Lights.ListAsync(selector, CancellationToken.None);

		var table = new Table();
		table.AddColumn("Label");
		table.AddColumn("ID");
		table.AddColumn("Power");
		table.AddColumn("Brightness");
		table.AddColumn("Color");
		table.AddColumn("Connected");

		if (verbose)
		{
			table.AddColumn("Group");
			table.AddColumn("Location");
		}

		foreach (var light in lights)
		{
			var row = new List<string>
			{
				light.Label,
				light.Id,
				light.IsOn ? "[green]On[/]" : "[dim]Off[/]",
				$"{light.Brightness:P0}",
				light.Color?.ToString() ?? "N/A",
				light.IsConnected ? "[green]Yes[/]" : "[red]No[/]"
			};

			if (verbose)
			{
				row.Add(light.GroupName);
				row.Add(light.LocationName);
			}

			table.AddRow(row.ToArray());
		}

		AnsiConsole.Write(table);
		AnsiConsole.MarkupLine($"[dim]Total: {lights.Count} lights[/]");
	}

	private static async Task ListGroups(LifxClient client, Selector selector, bool verbose)
	{
		var groups = await client.Lights.ListGroupsAsync(selector, CancellationToken.None);

		var table = new Table();
		table.AddColumn("Name");
		table.AddColumn("ID");
		table.AddColumn("Lights");

		foreach (var group in groups)
		{
			table.AddRow(group.Label, group.Id, group.Count().ToString());
		}

		AnsiConsole.Write(table);
		AnsiConsole.MarkupLine($"[dim]Total: {groups.Count} groups[/]");
	}

	private static async Task ListLocations(LifxClient client, Selector selector, bool verbose)
	{
		var locations = await client.Lights.ListLocationsAsync(selector, CancellationToken.None);

		var table = new Table();
		table.AddColumn("Name");
		table.AddColumn("ID");
		table.AddColumn("Lights");

		foreach (var location in locations)
		{
			table.AddRow(location.Label, location.Id, location.Count().ToString());
		}

		AnsiConsole.Write(table);
		AnsiConsole.MarkupLine($"[dim]Total: {locations.Count} locations[/]");
	}

	private static Command CreateOnCommand()
	{
		var command = new Command("on", "Turn lights on");

		var selectorArg = new Argument<string>(
			"selector",
			getDefaultValue: () => "all",
			description: "Which lights to turn on");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(selectorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (string? token, bool verbose, string selector, double duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new SetStateRequest
			{
				Power = PowerState.On,
				Duration = duration
			};

			await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Turned on lights: {selector}");
		},
		new TokenBinder(),
		new VerboseBinder(),
		selectorArg,
		durationOption);

		return command;
	}

	private static Command CreateOffCommand()
	{
		var command = new Command("off", "Turn lights off");

		var selectorArg = new Argument<string>(
			"selector",
			getDefaultValue: () => "all",
			description: "Which lights to turn off");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(selectorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (string? token, bool verbose, string selector, double duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new SetStateRequest
			{
				Power = PowerState.Off,
				Duration = duration
			};

			await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Turned off lights: {selector}");
		},
		new TokenBinder(),
		new VerboseBinder(),
		selectorArg,
		durationOption);

		return command;
	}

	private static Command CreateToggleCommand()
	{
		var command = new Command("toggle", "Toggle lights power");

		var selectorArg = new Argument<string>(
			"selector",
			getDefaultValue: () => "all",
			description: "Which lights to toggle");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(selectorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (string? token, bool verbose, string selector, double duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new TogglePowerRequest
			{
				Duration = duration
			};

			await client.Lights.TogglePowerAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Toggled power: {selector}");
		},
		new TokenBinder(),
		new VerboseBinder(),
		selectorArg,
		durationOption);

		return command;
	}

	private static Command CreateColorCommand()
	{
		var command = new Command("color", "Set light color");

		var selectorArg = new Argument<string>(
			"selector",
			getDefaultValue: () => "all",
			description: "Which lights to change");

		var colorArg = new Argument<string>(
			"color",
			description: "Color (name, rgb:R,G,B, or hue:H saturation:S brightness:B)");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(selectorArg);
		command.AddArgument(colorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (string? token, bool verbose, string selector, string color, double duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new SetStateRequest
			{
				Color = color,
				Duration = duration
			};

			await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Set color to '{color}': {selector}");
		},
		new TokenBinder(),
		new VerboseBinder(),
		selectorArg,
		colorArg,
		durationOption);

		return command;
	}

	private static Command CreateBrightnessCommand()
	{
		var command = new Command("brightness", "Set light brightness");

		var selectorArg = new Argument<string>(
			"selector",
			getDefaultValue: () => "all",
			description: "Which lights to change");

		var brightnessArg = new Argument<double>(
			"brightness",
			description: "Brightness level (0.0 to 1.0)");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(selectorArg);
		command.AddArgument(brightnessArg);
		command.AddOption(durationOption);

		command.SetHandler(async (string? token, bool verbose, string selector, double brightness, double duration) =>
		{
			if (brightness < 0 || brightness > 1)
			{
				AnsiConsole.MarkupLine("[red]Brightness must be between 0.0 and 1.0[/]");
				return;
			}

			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new SetStateRequest
			{
				Brightness = brightness,
				Duration = duration
			};

			await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Set brightness to {brightness:P0}: {selector}");
		},
		new TokenBinder(),
		new VerboseBinder(),
		selectorArg,
		brightnessArg,
		durationOption);

		return command;
	}
}

// Helper classes for binding global options
internal class TokenBinder : System.CommandLine.Binding.BinderBase<string?>
{
	protected override string? GetBoundValue(System.CommandLine.Binding.BindingContext bindingContext)
	{
		return bindingContext.ParseResult.GetValueForOption(
			bindingContext.ParseResult.RootCommandResult.Command.Options
				.OfType<Option<string?>>()
				.FirstOrDefault(o => o.HasAlias("--token")));
	}
}

internal class VerboseBinder : System.CommandLine.Binding.BinderBase<bool>
{
	protected override bool GetBoundValue(System.CommandLine.Binding.BindingContext bindingContext)
	{
		var option = bindingContext.ParseResult.RootCommandResult.Command.Options
			.OfType<Option<bool>>()
			.FirstOrDefault(o => o.HasAlias("--verbose"));
		
		return option != null && bindingContext.ParseResult.GetValueForOption(option);
	}
}
