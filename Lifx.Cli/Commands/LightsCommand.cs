using Lifx.Api;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the LightsCommand type.
/// </summary>
public static partial class LightsCommand
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
	/// Builds one subcommand that acts on a selection of lights: it resolves a cloud client from
	/// the global token and runs the action. The precondition runs before the client is resolved,
	/// so bad input is reported as such rather than as a missing API token.
	/// </summary>
	private static Command CreateSelectionCommand(
		string name,
		string description,
		Argument<string> selectorArgument,
		Argument[] extraArguments,
		Option[] options,
		LightsPrecondition precondition,
		LightsAction action)
		=> TargetCommandBuilder.Build(
			name,
			description,
			selectorArgument,
			extraArguments,
			options,
			precondition.Invoke,
			async (parseResult, selector, cancellationToken) =>
			{
				var token = parseResult.GetValue(GlobalOptions.Token);

				var factory = new LifxClientFactory();
				using var client = factory.CreateCloudClient(token);

				await action(client, SelectorParser.ParseSelector(selector), parseResult, cancellationToken);
			});

	private static Argument<string> CreateSelectorArgument(string description)
		=> new("selector")
		{
			Description = description,
			DefaultValueFactory = _ => "all"
		};

	/// <summary>
	/// Builds a light-state subcommand: one selector, optional extra arguments and a transition
	/// duration, differing only in how the request is built and sent and what is reported.
	/// </summary>
	private static Command CreateStateCommand(
		(string Name, string Description, string SelectorDescription) identity,
		Argument[] extraArguments,
		Func<ParseResult, double, bool> precondition,
		Func<ParseResult, double, object> buildRequest,
		Func<ILifxClient, Selector, object, CancellationToken, Task> send,
		Func<ParseResult, string, string> confirmation)
	{
		var selectorArgument = CreateSelectorArgument(identity.SelectorDescription);
		var durationOption = TargetCommandBuilder.CreateDurationOption();

		return CreateSelectionCommand(
			identity.Name,
			identity.Description,
			selectorArgument,
			extraArguments,
			[durationOption],
			parseResult => precondition(parseResult, parseResult.GetValue(durationOption)),
			async (client, selector, parseResult, cancellationToken) =>
			{
				var request = buildRequest(parseResult, parseResult.GetValue(durationOption));
				await send(client, selector, request, cancellationToken);
				AnsiConsole.MarkupLine($"[green]✓[/] {confirmation(parseResult, parseResult.GetValue(selectorArgument)!)}");
			});
	}

	private static Task SetState(ILifxClient client, Selector selector, object request, CancellationToken cancellationToken)
		=> client.Lights.SetStateAsync(selector, (SetStateRequest)request, cancellationToken);

	private static Command CreatePowerCommand(string verb, Func<double, SetStateRequest> build)
		=> CreateStateCommand(
			(verb, $"Turn lights {verb}", $"Which lights to turn {verb}"),
			[],
			(_, _) => true,
			(_, duration) => build(duration),
			SetState,
			(_, selector) => $"Turned {verb} lights: {selector}");

	private static Command CreateOnCommand()
		=> CreatePowerCommand("on", LightsHandler.BuildOnRequest);

	private static Command CreateOffCommand()
		=> CreatePowerCommand("off", LightsHandler.BuildOffRequest);

	private static Command CreateToggleCommand()
		=> CreateStateCommand(
			("toggle", "Toggle lights power", "Which lights to toggle"),
			[],
			(_, _) => true,
			(_, duration) => LightsHandler.BuildToggleRequest(duration),
			(client, selector, request, cancellationToken)
				=> client.Lights.TogglePowerAsync(selector, (TogglePowerRequest)request, cancellationToken),
			(_, selector) => $"Toggled power: {selector}");

	private static Command CreateColorCommand()
	{
		var colorArgument = new Argument<string>("color")
		{
			Description = "Color (name, rgb:R,G,B, or hue:H saturation:S brightness:B)"
		};

		return CreateStateCommand(
			("color", "Set light color", "Which lights to change"),
			[colorArgument],
			(_, _) => true,
			(parseResult, duration) => LightsHandler.BuildColorRequest(parseResult.GetValue(colorArgument)!, duration),
			SetState,
			(parseResult, selector) => $"Set color to '{parseResult.GetValue(colorArgument)}': {selector}");
	}

	private static Command CreateBrightnessCommand()
	{
		var brightnessArgument = new Argument<double>("brightness")
		{
			Description = "Brightness level (0.0 to 1.0)"
		};

		// Checked before a client is resolved: an out-of-range brightness must report itself
		// rather than a missing API token.
		return CreateStateCommand(
			("brightness", "Set light brightness", "Which lights to change"),
			[brightnessArgument],
			(parseResult, duration) => TryValidateBrightness(parseResult.GetValue(brightnessArgument), duration),
			(parseResult, duration) => LightsHandler.BuildBrightnessRequest(parseResult.GetValue(brightnessArgument), duration),
			SetState,
			(parseResult, selector) => $"Set brightness to {parseResult.GetValue(brightnessArgument):P0}: {selector}");
	}
}
