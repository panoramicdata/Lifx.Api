using System.CommandLine;
using Lifx.Api;
using Lifx.Api.Models.Cloud.Requests;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class ScenesCommand
{
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

		command.SetHandler(async (string? token, bool verbose) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var scenes = await client.Scenes.ListScenesAsync(CancellationToken.None);

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
		}, new TokenBinder(), new VerboseBinder());

		return command;
	}

	private static Command CreateActivateCommand()
	{
		var command = new Command("activate", "Activate a scene");

		var sceneArg = new Argument<string>("scene", description: "Scene name or UUID");
		var durationOption = new Option<double>(["--duration", "-d"], () => 1.0);
		var fastOption = new Option<bool>(["--fast"], () => false);

		command.AddArgument(sceneArg);
		command.AddOption(durationOption);
		command.AddOption(fastOption);

		command.SetHandler(async (string? token, string scene, double duration, bool fast) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			// Try to find scene by name or UUID
			var scenes = await client.Scenes.ListScenesAsync(CancellationToken.None);
			var targetScene = scenes.FirstOrDefault(s => 
				s.Name.Equals(scene, StringComparison.OrdinalIgnoreCase) || 
				s.Uuid.Equals(scene, StringComparison.OrdinalIgnoreCase));

			if (targetScene == null)
			{
				AnsiConsole.MarkupLine($"[red]Scene not found: {scene}[/]");
				return;
			}

			var request = new ActivateSceneRequest
			{
				Duration = duration,
				Fast = fast
			};

			await client.Scenes.ActivateSceneAsync($"scene_id:{targetScene.Uuid}", request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Activated scene: {targetScene.Name}");
		}, new TokenBinder(), sceneArg, durationOption, fastOption);

		return command;
	}
}
