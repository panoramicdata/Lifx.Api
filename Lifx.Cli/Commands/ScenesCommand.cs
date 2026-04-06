using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the ScenesCommand type.
/// </summary>
public static class ScenesCommand
{
	/// <summary>
	/// Performs Create operation.
	/// </summary>
	public static Command Create()
	{
		var command = new Command("scenes", "Manage and activate scenes")
		{
			CreateListCommand(),
			CreateActivateCommand()
		};

		return command;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List all scenes");

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			var token = parseResult.GetValue(GlobalOptions.Token);
			var verbose = parseResult.GetValue(GlobalOptions.Verbose);

			var factory = new LifxClientFactory();
			using var client = factory.CreateCloudClient(token);

			var scenes = await client.Scenes.ListScenesAsync(cancellationToken);

			var table = new Table();
			table.AddColumn("Name");
			table.AddColumn("UUID");

			if (verbose)
			{
				table.AddColumn("Lights");
			}

			foreach (var scene in scenes)
			{
				if (verbose)
				{
					table.AddRow(scene.Name, scene.Uuid, scene.States?.Count.ToString() ?? "0");
				}
				else
				{
					table.AddRow(scene.Name, scene.Uuid);
				}
			}

			AnsiConsole.Write(table);
			AnsiConsole.MarkupLine($"[dim]Total: {scenes.Count} scenes[/]");
		});

		return command;
	}

	private static Command CreateActivateCommand()
	{
		var command = new Command("activate", "Activate a scene");

		var sceneArg = new Argument<string>("scene")
		{
			Description = "Scene name or UUID"
		};
		var durationOption = new Option<double>("--duration", "-d")
		{
			DefaultValueFactory = _ => 1.0
		};
		var fastOption = new Option<bool>("--fast")
		{
			DefaultValueFactory = _ => false
		};

		command.Arguments.Add(sceneArg);
		command.Options.Add(durationOption);
		command.Options.Add(fastOption);

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			var token = parseResult.GetValue(GlobalOptions.Token);
			var scene = parseResult.GetValue(sceneArg);
			var duration = parseResult.GetValue(durationOption);
			var fast = parseResult.GetValue(fastOption);

			var factory = new LifxClientFactory();
			using var client = factory.CreateCloudClient(token);

			var scenes = await client.Scenes.ListScenesAsync(cancellationToken);
			var targetScene = ScenesHandler.FindScene(scenes, scene!);

			if (targetScene == null)
			{
				AnsiConsole.MarkupLine($"[red]Scene not found: {scene}[/]");
				return;
			}

			var request = ScenesHandler.BuildActivateRequest(duration, fast);
			var sceneSelector = ScenesHandler.BuildSceneSelector(targetScene.Uuid);

			await client.Scenes.ActivateSceneAsync(sceneSelector, request, cancellationToken);
			AnsiConsole.MarkupLine($"[green]✓[/] Activated scene: {targetScene.Name}");
		});

		return command;
	}
}
