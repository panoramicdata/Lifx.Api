using Lifx.Api;
using Lifx.Api.Models.Cloud.Requests;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

public static class EffectsCommand
{
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

	private static Command CreateBreatheCommand()
	{
		var command = new Command("breathe", "Breathe effect");

		var selectorArg = new Argument<string>("selector", () => "all");
		var colorOption = new Option<string>(["--color", "-c"], () => "blue");
		var periodOption = new Option<double>(["--period", "-p"], () => 2.0);
		var cyclesOption = new Option<double>(["--cycles"], () => 5.0);

		command.AddArgument(selectorArg);
		command.AddOption(colorOption);
		command.AddOption(periodOption);
		command.AddOption(cyclesOption);

		command.SetHandler(async (token, selector, color, period, cycles) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new BreatheEffectRequest
			{
				Color = color,
				Period = period,
				Cycles = cycles,
				PowerOn = true
			};

			await client.Effects.BreatheAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started breathe effect on {selector}");
		}, new TokenBinder(), selectorArg, colorOption, periodOption, cyclesOption);

		return command;
	}

	private static Command CreatePulseCommand()
	{
		var command = new Command("pulse", "Pulse effect");

		var selectorArg = new Argument<string>("selector", () => "all");
		var colorOption = new Option<string>(["--color", "-c"], () => "red");
		var periodOption = new Option<double>(["--period", "-p"], () => 1.0);
		var cyclesOption = new Option<double>(["--cycles"], () => 5.0);

		command.AddArgument(selectorArg);
		command.AddOption(colorOption);
		command.AddOption(periodOption);
		command.AddOption(cyclesOption);

		command.SetHandler(async (token, selector, color, period, cycles) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new PulseEffectRequest
			{
				Color = color,
				Period = period,
				Cycles = cycles,
				PowerOn = true
			};

			await client.Effects.PulseAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started pulse effect on {selector}");
		}, new TokenBinder(), selectorArg, colorOption, periodOption, cyclesOption);

		return command;
	}

	private static Command CreateMorphCommand()
	{
		var command = new Command("morph", "Morph effect");

		var selectorArg = new Argument<string>("selector", () => "all");
		var periodOption = new Option<double>(["--period", "-p"], () => 3.0);
		var durationOption = new Option<double>(["--duration", "-d"], () => 30.0);

		command.AddArgument(selectorArg);
		command.AddOption(periodOption);
		command.AddOption(durationOption);

		command.SetHandler(async (token, selector, period, duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new MorphEffectRequest
			{
				Period = period,
				Duration = duration,
				PowerOn = true
			};

			await client.Effects.MorphAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started morph effect on {selector}");
		}, new TokenBinder(), selectorArg, periodOption, durationOption);

		return command;
	}

	private static Command CreateFlameCommand()
	{
		var command = new Command("flame", "Flame effect");

		var selectorArg = new Argument<string>("selector", () => "all");
		var periodOption = new Option<double>(["--period", "-p"], () => 5.0);
		var durationOption = new Option<double>(["--duration", "-d"], () => 60.0);

		command.AddArgument(selectorArg);
		command.AddOption(periodOption);
		command.AddOption(durationOption);

		command.SetHandler(async (token, selector, period, duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new FlameEffectRequest
			{
				Period = period,
				Duration = duration,
				PowerOn = true
			};

			await client.Effects.FlameAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started flame effect on {selector}");
		}, new TokenBinder(), selectorArg, periodOption, durationOption);

		return command;
	}

	private static Command CreateMoveCommand()
	{
		var command = new Command("move", "Move effect (multi-zone)");

		var selectorArg = new Argument<string>("selector", () => "all");
		var directionOption = new Option<string>(["--direction", "-d"], () => "forward");
		var periodOption = new Option<double>(["--period", "-p"], () => 2.0);

		command.AddArgument(selectorArg);
		command.AddOption(directionOption);
		command.AddOption(periodOption);

		command.SetHandler(async (token, selector, direction, period) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new MoveEffectRequest
			{
				Direction = direction,
				Period = period,
				PowerOn = true
			};

			await client.Effects.MoveAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started move effect on {selector}");
		}, new TokenBinder(), selectorArg, directionOption, periodOption);

		return command;
	}

	private static Command CreateCloudsCommand()
	{
		var command = new Command("clouds", "Clouds effect (multi-zone)");

		var selectorArg = new Argument<string>("selector", () => "all");
		var durationOption = new Option<double>(["--duration", "-d"], () => 120.0);

		command.AddArgument(selectorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (token, selector, duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new CloudsEffectRequest
			{
				Duration = duration,
				PowerOn = true
			};

			await client.Effects.CloudsAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started clouds effect on {selector}");
		}, new TokenBinder(), selectorArg, durationOption);

		return command;
	}

	private static Command CreateSunriseCommand()
	{
		var command = new Command("sunrise", "Sunrise effect");

		var selectorArg = new Argument<string>("selector", () => "all");
		var durationOption = new Option<double>(["--duration", "-d"], () => 300.0);

		command.AddArgument(selectorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (token, selector, duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new SunriseEffectRequest
			{
				Duration = duration
			};

			await client.Effects.SunriseAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started sunrise effect on {selector}");
		}, new TokenBinder(), selectorArg, durationOption);

		return command;
	}

	private static Command CreateSunsetCommand()
	{
		var command = new Command("sunset", "Sunset effect");

		var selectorArg = new Argument<string>("selector", () => "all");
		var durationOption = new Option<double>(["--duration", "-d"], () => 300.0);

		command.AddArgument(selectorArg);
		command.AddOption(durationOption);

		command.SetHandler(async (token, selector, duration) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new SunsetEffectRequest
			{
				Duration = duration
			};

			await client.Effects.SunsetAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Started sunset effect on {selector}");
		}, new TokenBinder(), selectorArg, durationOption);

		return command;
	}

	private static Command CreateOffCommand()
	{
		var command = new Command("off", "Stop all effects");

		var selectorArg = new Argument<string>("selector", () => "all");
		var powerOffOption = new Option<bool>(["--power-off"], () => false);

		command.AddArgument(selectorArg);
		command.AddOption(powerOffOption);

		command.SetHandler(async (token, selector, powerOff) =>
		{
			var apiToken = ConfigManager.GetApiToken(token);
			using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

			var request = new EffectsOffRequest
			{
				PowerOff = powerOff
			};

			await client.Effects.OffAsync(SelectorParser.ParseSelector(selector), request, CancellationToken.None);
			AnsiConsole.MarkupLine($"[green]?[/] Stopped effects on {selector}");
		}, new TokenBinder(), selectorArg, powerOffOption);

		return command;
	}
}
