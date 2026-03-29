using Lifx.Api;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Requests;
using Spectre.Console;
using System.CommandLine;

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

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var selectorObj = SelectorParser.ParseSelector(selector);

            switch (type.ToLowerInvariant())
            {
                case "lights":
                    await ListLights(client, selectorObj, verbose);
                    break;
                case "groups":
                    await ListGroups(client, selectorObj);
                    break;
                case "locations":
                    await ListLocations(client, selectorObj);
                    break;
                default:
                    AnsiConsole.MarkupLine($"[red]Unknown type: {type}[/]");
                    break;
            }
        });

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

    private static async Task ListGroups(LifxClient client, Selector selector)
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

    private static async Task ListLocations(LifxClient client, Selector selector)
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

        var selectorArg = new Argument<string>("selector")
        {
            Description = "Which lights to turn on",
            DefaultValueFactory = _ => "all"
        };

        var durationOption = new Option<double>("--duration", "-d")
        {
            Description = "Transition duration in seconds",
            DefaultValueFactory = _ => 1.0
        };

        command.Arguments.Add(selectorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new SetStateRequest
            {
                Power = PowerState.On,
                Duration = duration
            };

            await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Turned on lights: {selector}");
        });

        return command;
    }

    private static Command CreateOffCommand()
    {
        var command = new Command("off", "Turn lights off");

        var selectorArg = new Argument<string>("selector")
        {
            Description = "Which lights to turn off",
            DefaultValueFactory = _ => "all"
        };

        var durationOption = new Option<double>("--duration", "-d")
        {
            Description = "Transition duration in seconds",
            DefaultValueFactory = _ => 1.0
        };

        command.Arguments.Add(selectorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new SetStateRequest
            {
                Power = PowerState.Off,
                Duration = duration
            };

            await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Turned off lights: {selector}");
        });

        return command;
    }

    private static Command CreateToggleCommand()
    {
        var command = new Command("toggle", "Toggle lights power");

        var selectorArg = new Argument<string>("selector")
        {
            Description = "Which lights to toggle",
            DefaultValueFactory = _ => "all"
        };

        var durationOption = new Option<double>("--duration", "-d")
        {
            Description = "Transition duration in seconds",
            DefaultValueFactory = _ => 1.0
        };

        command.Arguments.Add(selectorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new TogglePowerRequest
            {
                Duration = duration
            };

            await client.Lights.TogglePowerAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Toggled power: {selector}");
        });

        return command;
    }

    private static Command CreateColorCommand()
    {
        var command = new Command("color", "Set light color");

        var selectorArg = new Argument<string>("selector")
        {
            Description = "Which lights to change",
            DefaultValueFactory = _ => "all"
        };

        var colorArg = new Argument<string>("color")
        {
            Description = "Color (name, rgb:R,G,B, or hue:H saturation:S brightness:B)"
        };

        var durationOption = new Option<double>("--duration", "-d")
        {
            Description = "Transition duration in seconds",
            DefaultValueFactory = _ => 1.0
        };

        command.Arguments.Add(selectorArg);
        command.Arguments.Add(colorArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var color = parseResult.GetValue(colorArg)!;
            var duration = parseResult.GetValue(durationOption);

            var apiToken = ConfigManager.GetApiToken(token);
            using var client = new LifxClient(new LifxClientOptions { ApiToken = apiToken });

            var request = new SetStateRequest
            {
                Color = color,
                Duration = duration
            };

            await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Set color to '{color}': {selector}");
        });

        return command;
    }

    private static Command CreateBrightnessCommand()
    {
        var command = new Command("brightness", "Set light brightness");

        var selectorArg = new Argument<string>("selector")
        {
            Description = "Which lights to change",
            DefaultValueFactory = _ => "all"
        };

        var brightnessArg = new Argument<double>("brightness")
        {
            Description = "Brightness level (0.0 to 1.0)"
        };

        var durationOption = new Option<double>("--duration", "-d")
        {
            Description = "Transition duration in seconds",
            DefaultValueFactory = _ => 1.0
        };

        command.Arguments.Add(selectorArg);
        command.Arguments.Add(brightnessArg);
        command.Options.Add(durationOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var token = parseResult.GetValue(GlobalOptions.Token);
            var selector = parseResult.GetValue(selectorArg)!;
            var brightness = parseResult.GetValue(brightnessArg);
            var duration = parseResult.GetValue(durationOption);

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

            await client.Lights.SetStateAsync(SelectorParser.ParseSelector(selector), request, cancellationToken);
            AnsiConsole.MarkupLine($"[green]✓[/] Set brightness to {brightness:P0}: {selector}");
        });

        return command;
    }
}
