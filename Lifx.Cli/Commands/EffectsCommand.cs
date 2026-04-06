using Lifx.Api;
using Lifx.Api.Models.Cloud.Requests;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the EffectsCommand type.
/// </summary>
public static class EffectsCommand
{
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

    private static Command CreateBreatheCommand()
    {
        var command = new Command("breathe", "Breathe effect");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var colorOption = new Option<string>("--color", "-c") { DefaultValueFactory = _ => "blue" };
        var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 2.0 };
        var cyclesOption = new Option<double>("--cycles") { DefaultValueFactory = _ => 5.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(colorOption);
        command.Options.Add(periodOption);
        command.Options.Add(cyclesOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var color = parseResult.GetValue(colorOption)!;
            var period = parseResult.GetValue(periodOption);
            var cycles = parseResult.GetValue(cyclesOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new BreatheEffectRequest
            {
                Color = color,
                Period = period,
                Cycles = cycles,
                PowerOn = true
            };

            await client.Effects.BreatheAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started breathe effect on {selector}");
        });

        return command;
    }

    private static Command CreatePulseCommand()
    {
        var command = new Command("pulse", "Pulse effect");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var colorOption = new Option<string>("--color", "-c") { DefaultValueFactory = _ => "red" };
        var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 1.0 };
        var cyclesOption = new Option<double>("--cycles") { DefaultValueFactory = _ => 5.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(colorOption);
        command.Options.Add(periodOption);
        command.Options.Add(cyclesOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var color = parseResult.GetValue(colorOption)!;
            var period = parseResult.GetValue(periodOption);
            var cycles = parseResult.GetValue(cyclesOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new PulseEffectRequest
            {
                Color = color,
                Period = period,
                Cycles = cycles,
                PowerOn = true
            };

            await client.Effects.PulseAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started pulse effect on {selector}");
        });

        return command;
    }

    private static Command CreateMorphCommand()
    {
        var command = new Command("morph", "Morph effect");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 3.0 };
        var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 30.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(periodOption);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var period = parseResult.GetValue(periodOption);
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new MorphEffectRequest
            {
                Period = period,
                Duration = duration,
                PowerOn = true
            };

            await client.Effects.MorphAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started morph effect on {selector}");
        });

        return command;
    }

    private static Command CreateFlameCommand()
    {
        var command = new Command("flame", "Flame effect");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 5.0 };
        var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 60.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(periodOption);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var period = parseResult.GetValue(periodOption);
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new FlameEffectRequest
            {
                Period = period,
                Duration = duration,
                PowerOn = true
            };

            await client.Effects.FlameAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started flame effect on {selector}");
        });

        return command;
    }

    private static Command CreateMoveCommand()
    {
        var command = new Command("move", "Move effect (multi-zone)");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var directionOption = new Option<string>("--direction", "-d") { DefaultValueFactory = _ => "forward" };
        var periodOption = new Option<double>("--period", "-p") { DefaultValueFactory = _ => 2.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(directionOption);
        command.Options.Add(periodOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var direction = parseResult.GetValue(directionOption)!;
            var period = parseResult.GetValue(periodOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new MoveEffectRequest
            {
                Direction = direction,
                Period = period,
                PowerOn = true
            };

            await client.Effects.MoveAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started move effect on {selector}");
        });

        return command;
    }

    private static Command CreateCloudsCommand()
    {
        var command = new Command("clouds", "Clouds effect (multi-zone)");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 120.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new CloudsEffectRequest
            {
                Duration = duration,
                PowerOn = true
            };

            await client.Effects.CloudsAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started clouds effect on {selector}");
        });

        return command;
    }

    private static Command CreateSunriseCommand()
    {
        var command = new Command("sunrise", "Sunrise effect");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 300.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new SunriseEffectRequest
            {
                Duration = duration
            };

            await client.Effects.SunriseAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started sunrise effect on {selector}");
        });

        return command;
    }

    private static Command CreateSunsetCommand()
    {
        var command = new Command("sunset", "Sunset effect");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var durationOption = new Option<double>("--duration", "-d") { DefaultValueFactory = _ => 300.0 };

        command.Arguments.Add(selectorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new SunsetEffectRequest
            {
                Duration = duration
            };

            await client.Effects.SunsetAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Started sunset effect on {selector}");
        });

        return command;
    }

    private static Command CreateOffCommand()
    {
        var command = new Command("off", "Stop all effects");

        var selectorArg = new Argument<string>("selector") { DefaultValueFactory = _ => "all" };
        var powerOffOption = new Option<bool>("--power-off") { DefaultValueFactory = _ => false };

        command.Arguments.Add(selectorArg);
        command.Options.Add(powerOffOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var powerOff = parseResult.GetValue(powerOffOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new EffectsOffRequest
            {
                PowerOff = powerOff
            };

            await client.Effects.OffAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Stopped effects on {selector}");
        });

        return command;
    }
}
