using Lifx.Api;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Responses;
using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

public static partial class LightsCommand
{
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
		WriteCollectionTable(groups, "groups");
	}

	private static void WriteCollectionTable<T>(List<T> collections, string noun)
		where T : LightCollection
	{
		var table = new Table();
		table.AddColumn("Name");
		table.AddColumn("ID");
		table.AddColumn("Lights");

		foreach (var collection in collections)
		{
			table.AddRow(collection.Label, collection.Id, collection.Count().ToString());
		}

		AnsiConsole.Write(table);
		AnsiConsole.MarkupLine($"[dim]Total: {collections.Count} {noun}[/]");
	}

	private static async Task ListLocations(ILifxClient client, Selector selector, CancellationToken cancellationToken)
	{
		var locations = await client.Lights.ListLocationsAsync(selector, cancellationToken);
		WriteCollectionTable(locations, "locations");
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
