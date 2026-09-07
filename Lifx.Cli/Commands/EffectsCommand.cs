using Lifx.Api;
using Lifx.Api.Models.Cloud;
using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the EffectsCommand type.
/// </summary>
public static class EffectsCommand
{
	/// <summary>
	/// Starts one effect against the resolved selector.
	/// </summary>
	private delegate Task EffectAction(
		ILifxClient client,
		Selector selector,
		ParseResult parseResult,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs Create operation.
	/// </summary>
	public static Command Create()
	{
		var command = new Command("effects", "Run visual effects on lights")
		{
			CreateBreatheCommand(),
			CreatePulseCommand(),
			CreateMorphCommand(),
			CreateFlameCommand(),
			CreateMoveCommand(),
			CreateCloudsCommand(),
			CreateSunriseCommand(),
			CreateSunsetCommand(),
			CreateOffCommand()
		};

		return command;
	}

	/// <summary>
	/// Builds one effect subcommand.
	/// </summary>
	/// <remarks>
	/// Every effect takes the same selector argument, resolves a cloud client the same way and
	/// reports the same way; only its options and the call it makes differ. Those are what the
	/// caller supplies, so the shared half lives here once rather than nine times over.
	/// </remarks>
	private static Command CreateEffectCommand(
		string name,
		string description,
		string completedMessage,
		Argument<string> selectorArgument,
		Option[] options,
		EffectAction action)
	{
		var command = new Command(name, description);

		command.Arguments.Add(selectorArgument);
		foreach (var option in options)
		{
			command.Options.Add(option);
		}

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			var token = parseResult.GetValue(GlobalOptions.Token);
			var selector = parseResult.GetValue(selectorArgument)!;

			var factory = new LifxClientFactory();
			using var client = factory.CreateCloudClient(token);

			await action(client, SelectorParser.ParseSelector(selector), parseResult, cancellationToken);
			AnsiConsole.MarkupLine($"[green]✓[/] {completedMessage} on {selector}");
		});

		return command;
	}

	private static Argument<string> CreateSelectorArgument()
		=> new("selector") { DefaultValueFactory = _ => "all" };

	private static Command CreateBreatheCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var colorOption = new Option<string>("--color", "-c") { DefaultValueFactory = _ => "blue" };
		var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 2.0 };
		var cyclesOption = new Option<double>("--cycles") { DefaultValueFactory = _ => 5.0 };

		return CreateEffectCommand(
			"breathe",
			"Breathe effect",
			"Started breathe effect",
			selectorArgument,
			[colorOption, periodOption, cyclesOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.BreatheAsync(
				selector,
				EffectsHandler.BuildBreatheRequest(
					parseResult.GetValue(colorOption)!,
					parseResult.GetValue(periodOption),
					parseResult.GetValue(cyclesOption)),
				cancellationToken));
	}

	private static Command CreatePulseCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var colorOption = new Option<string>("--color", "-c") { DefaultValueFactory = _ => "red" };
		var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 1.0 };
		var cyclesOption = new Option<double>("--cycles") { DefaultValueFactory = _ => 5.0 };

		return CreateEffectCommand(
			"pulse",
			"Pulse effect",
			"Started pulse effect",
			selectorArgument,
			[colorOption, periodOption, cyclesOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.PulseAsync(
				selector,
				EffectsHandler.BuildPulseRequest(
					parseResult.GetValue(colorOption)!,
					parseResult.GetValue(periodOption),
					parseResult.GetValue(cyclesOption)),
				cancellationToken));
	}

	private static Command CreateMorphCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 3.0 };
		var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 30.0 };

		return CreateEffectCommand(
			"morph",
			"Morph effect",
			"Started morph effect",
			selectorArgument,
			[periodOption, durationOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.MorphAsync(
				selector,
				EffectsHandler.BuildMorphRequest(
					parseResult.GetValue(periodOption),
					parseResult.GetValue(durationOption)),
				cancellationToken));
	}

	private static Command CreateFlameCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 5.0 };
		var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 60.0 };

		return CreateEffectCommand(
			"flame",
			"Flame effect",
			"Started flame effect",
			selectorArgument,
			[periodOption, durationOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.FlameAsync(
				selector,
				EffectsHandler.BuildFlameRequest(
					parseResult.GetValue(periodOption),
					parseResult.GetValue(durationOption)),
				cancellationToken));
	}

	private static Command CreateMoveCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var directionOption = new Option<string>("--direction", "-d") { DefaultValueFactory = _ => "forward" };
		var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 2.0 };

		return CreateEffectCommand(
			"move",
			"Move effect (multi-zone)",
			"Started move effect",
			selectorArgument,
			[directionOption, periodOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.MoveAsync(
				selector,
				EffectsHandler.BuildMoveRequest(
					parseResult.GetValue(directionOption)!,
					parseResult.GetValue(periodOption)),
				cancellationToken));
	}

	private static Command CreateCloudsCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 120.0 };

		return CreateEffectCommand(
			"clouds",
			"Clouds effect (multi-zone)",
			"Started clouds effect",
			selectorArgument,
			[durationOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.CloudsAsync(
				selector,
				EffectsHandler.BuildCloudsRequest(parseResult.GetValue(durationOption)),
				cancellationToken));
	}

	private static Command CreateSunriseCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 300.0 };

		return CreateEffectCommand(
			"sunrise",
			"Sunrise effect",
			"Started sunrise effect",
			selectorArgument,
			[durationOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.SunriseAsync(
				selector,
				EffectsHandler.BuildSunriseRequest(parseResult.GetValue(durationOption)),
				cancellationToken));
	}

	private static Command CreateSunsetCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 300.0 };

		return CreateEffectCommand(
			"sunset",
			"Sunset effect",
			"Started sunset effect",
			selectorArgument,
			[durationOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.SunsetAsync(
				selector,
				EffectsHandler.BuildSunsetRequest(parseResult.GetValue(durationOption)),
				cancellationToken));
	}

	private static Command CreateOffCommand()
	{
		var selectorArgument = CreateSelectorArgument();
		var powerOffOption = new Option<bool>("--power-off") { DefaultValueFactory = _ => false };

		return CreateEffectCommand(
			"off",
			"Stop all effects",
			"Stopped effects",
			selectorArgument,
			[powerOffOption],
			(client, selector, parseResult, cancellationToken) => client.Effects.OffAsync(
				selector,
				EffectsHandler.BuildOffRequest(parseResult.GetValue(powerOffOption)),
				cancellationToken));
	}
}
