using Lifx.Api;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud;
using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the LightsCommand type.
/// </summary>
public static class LightsCommand
{
	/// <summary>
	/// Applies one operation to the selected lights.
	/// </summary>
	private delegate Task LightsAction(
		ILifxClient client,
		Selector selector,
		ParseResult parseResult,
		CancellationToken cancellationToken);

	/// <summary>
	/// Rejects bad input before a client is resolved. Returns false once a message has been shown.
	/// </summary>
	private delegate bool LightsPrecondition(ParseResult parseResult);

	/// <summary>
	/// Performs Create operation.
	/// </summary>
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

	/// <summary>
	/// Builds one subcommand that acts on a selection of lights, with nothing to check first.
	/// </summary>
	private static Command CreateSelectionCommand(
		string name,
		string description,
		Argument<string> selectorArgument,
		Argument[] extraArguments,
		Option[] options,
		LightsAction action)
		=> CreateSelectionCommand(
			name,
			description,
			selectorArgument,
			extraArguments,
			options,
			_ => true,
			action);

	/// <summary>
	/// Builds one subcommand that acts on a selection of lights.
	/// </summary>
	/// <remarks>
	/// Every one of these takes a selector, resolves a cloud client from the global token and
	/// reports the same way; only its arguments and the call it makes differ. The precondition runs
	/// before the client is resolved, so bad input is reported as such rather than as a missing
	/// API token.
	/// </remarks>
	private static Command CreateSelectionCommand(
		string name,
		string description,
		Argument<string> selectorArgument,
		Argument[] extraArguments,
		Option[] options,
		LightsPrecondition precondition,
		LightsAction action)
	{
		var command = new Command(name, description);

		command.Arguments.Add(selectorArgument);
		foreach (var argument in extraArguments)
		{
			command.Arguments.Add(argument);
		}

		foreach (var option in options)
		{
			command.Options.Add(option);
		}

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			if (!precondition(parseResult))
			{
				return;
			}

			var token = parseResult.GetValue(GlobalOptions.Token);
			var selector = parseResult.GetValue(selectorArgument)!;

			var factory = new LifxClientFactory();
			using var client = factory.CreateCloudClient(token);

			await action(client, SelectorParser.ParseSelector(selector), parseResult, cancellationToken);
		});

		return command;
	}

	private static Argument<string> CreateSelectorArgument(string description)
		=> new("selector")
		{
			Description = description,
			DefaultValueFactory = _ => "all"
		};

	private static Option<double> CreateDurationOption()
		=> new("--duration", "-d")
		{
			Description = "Transition duration in seconds",
			DefaultValueFactory = _ => 1.0
		};

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List all lights, groups, or locations");

		var typeOption = new Option<string>("--type", "-t")
		{
			Description = "Type to list (lights, groups, locations)",
			DefaultValueFactory = _ => "lights"
		};

		var selectorOption = new Option<string>("--selector", "-s")
		{
			Description = "Selector for filtering",
			DefaultValueFactory = _ => "all"
		};

		command.Options.Add(typeOption);
		command.Options.Add(selectorOption);

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			var token = parseResult.GetValue(GlobalOptions.Token);
			var verbose = parseResult.GetValue(GlobalOptions.Verbose);
			var type = parseResult.GetValue(typeOption)!;
			var selector = parseResult.GetValue(selectorOption)!;

			var factory = new LifxClientFactory();
			using var client = factory.CreateCloudClient(token);

			var selectorObj = SelectorParser.ParseSelector(selector);

			switch (type.ToLowerInvariant())
			{
				case "lights":
					await ListLights(client, selectorObj, verbose, cancellationToken);
					break;
				case "groups":
					await ListGroups(client, selectorObj, cancellationToken);
					break;
				case "locations":
					await ListLocations(client, selectorObj, cancellationToken);
					break;
				default:
					AnsiConsole.MarkupLine($"[red]Unknown type: {type}[/]");
					break;
			}
		});

		return command;
	}

	private static async Task ListLights(ILifxClient client, Selector selector, bool verbose, CancellationToken cancellationToken)
	{
		var lights = await client.Lights.ListAsync(selector, cancellationToken);

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

			table.AddRow([.. row]);
		}

		AnsiConsole.Write(table);
		AnsiConsole.MarkupLine($"[dim]Total: {lights.Count} lights[/]");
	}

	private static async Task ListGroups(ILifxClient client, Selector selector, CancellationToken cancellationToken)
	{
		var groups = await client.Lights.ListGroupsAsync(selector, cancellationToken);

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

	private static async Task ListLocations(ILifxClient client, Selector selector, CancellationToken cancellationToken)
	{
		var locations = await client.Lights.ListLocationsAsync(selector, cancellationToken);

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
		var selectorArgument = CreateSelectorArgument("Which lights to turn on");
		var durationOption = CreateDurationOption();

		return CreateSelectionCommand(
			"on",
			"Turn lights on",
			selectorArgument,
			[],
			[durationOption],
			async (client, selector, parseResult, cancellationToken) =>
			{
				var request = LightsHandler.BuildOnRequest(parseResult.GetValue(durationOption));
				await client.Lights.SetStateAsync(selector, request, cancellationToken);
				AnsiConsole.MarkupLine($"[green]✓[/] Turned on lights: {parseResult.GetValue(selectorArgument)}");
			});
	}

	private static Command CreateOffCommand()
	{
		var selectorArgument = CreateSelectorArgument("Which lights to turn off");
		var durationOption = CreateDurationOption();

		return CreateSelectionCommand(
			"off",
			"Turn lights off",
			selectorArgument,
			[],
			[durationOption],
			async (client, selector, parseResult, cancellationToken) =>
			{
				var request = LightsHandler.BuildOffRequest(parseResult.GetValue(durationOption));
				await client.Lights.SetStateAsync(selector, request, cancellationToken);
				AnsiConsole.MarkupLine($"[green]✓[/] Turned off lights: {parseResult.GetValue(selectorArgument)}");
			});
	}

	private static Command CreateToggleCommand()
	{
		var selectorArgument = CreateSelectorArgument("Which lights to toggle");
		var durationOption = CreateDurationOption();

		return CreateSelectionCommand(
			"toggle",
			"Toggle lights power",
			selectorArgument,
			[],
			[durationOption],
			async (client, selector, parseResult, cancellationToken) =>
			{
				var request = LightsHandler.BuildToggleRequest(parseResult.GetValue(durationOption));
				await client.Lights.TogglePowerAsync(selector, request, cancellationToken);
				AnsiConsole.MarkupLine($"[green]✓[/] Toggled power: {parseResult.GetValue(selectorArgument)}");
			});
	}

	private static Command CreateColorCommand()
	{
		var selectorArgument = CreateSelectorArgument("Which lights to change");
		var colorArgument = new Argument<string>("color")
		{
			Description = "Color (name, rgb:R,G,B, or hue:H saturation:S brightness:B)"
		};
		var durationOption = CreateDurationOption();

		return CreateSelectionCommand(
			"color",
			"Set light color",
			selectorArgument,
			[colorArgument],
			[durationOption],
			async (client, selector, parseResult, cancellationToken) =>
			{
				var color = parseResult.GetValue(colorArgument)!;
				var request = LightsHandler.BuildColorRequest(color, parseResult.GetValue(durationOption));
				await client.Lights.SetStateAsync(selector, request, cancellationToken);
				AnsiConsole.MarkupLine($"[green]✓[/] Set color to '{color}': {parseResult.GetValue(selectorArgument)}");
			});
	}

	private static Command CreateBrightnessCommand()
	{
		var selectorArgument = CreateSelectorArgument("Which lights to change");
		var brightnessArgument = new Argument<double>("brightness")
		{
			Description = "Brightness level (0.0 to 1.0)"
		};
		var durationOption = CreateDurationOption();

		return CreateSelectionCommand(
			"brightness",
			"Set light brightness",
			selectorArgument,
			[brightnessArgument],
			[durationOption],
			// Checked before a client is resolved, as it was originally: an out-of-range brightness
			// must report itself rather than a missing API token.
			parseResult => TryValidateBrightness(
				parseResult.GetValue(brightnessArgument),
				parseResult.GetValue(durationOption)),
			async (client, selector, parseResult, cancellationToken) =>
			{
				var brightness = parseResult.GetValue(brightnessArgument);
				var request = LightsHandler.BuildBrightnessRequest(brightness, parseResult.GetValue(durationOption));
				await client.Lights.SetStateAsync(selector, request, cancellationToken);
				AnsiConsole.MarkupLine($"[green]✓[/] Set brightness to {brightness:P0}: {parseResult.GetValue(selectorArgument)}");
			});
	}

	/// <summary>
	/// Reports an out-of-range brightness to the console and returns false.
	/// </summary>
	private static bool TryValidateBrightness(double brightness, double duration)
	{
		try
		{
			LightsHandler.BuildBrightnessRequest(brightness, duration);
			return true;
		}
		catch (ArgumentOutOfRangeException ex)
		{
			AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
			return false;
		}
	}
}
