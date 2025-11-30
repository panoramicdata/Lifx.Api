using Lifx.Api;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

public static class LanCommand
{
	public static Command Create()
	{
		var command = new Command("lan", "Control LIFX lights via LAN protocol (no API token needed)")
		{
			CreateDiscoverCommand(),
			CreateListCommand(),
			CreateLightsCommand()
		};

		command.Description =
			"Control LIFX lights via LAN protocol (no API token needed)" + Environment.NewLine +
			Environment.NewLine +
			"The LAN protocol allows you to control lights on your local network." + Environment.NewLine +
			"Works without internet connection and without API token." + Environment.NewLine +
			Environment.NewLine +
			"Subcommands:" + Environment.NewLine +
			"  discover  - Discover LIFX devices on local network" + Environment.NewLine +
			"  list      - List cached discovered devices" + Environment.NewLine +
			"  lights    - Control discovered lights (on/off/color)" + Environment.NewLine +
			Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"  lifx lan discover --timeout 10    # Discover for 10 seconds" + Environment.NewLine +
			"  lifx lan list                     # Show discovered devices" + Environment.NewLine +
			"  lifx lan lights on D0:73:D5:XX:XX:XX   # Turn on a light" + Environment.NewLine +
			"  lifx lan lights color D0:73:D5:XX:XX:XX 2700  # Set to warm white" + Environment.NewLine +
			Environment.NewLine +
			"Note: Devices must be on the same network as your computer.";

		return command;
	}

	private static Command CreateLightsCommand()
	{
		var command = new Command("lights", "Control LAN lights")
		{
			CreateLanOnCommand(),
			CreateLanOffCommand(),
			CreateLanColorCommand(),
			CreateLanStateCommand(),
			CreateLanRenameCommand()
		};

		return command;
	}

	private static Command CreateLanOnCommand()
	{
		var command = new Command("on", "Turn light on via LAN");

		var macArg = new Argument<string>(
			"mac-address",
			description: "MAC address of the light (e.g., D0:73:D5:12:34:56)");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(macArg);
		command.AddOption(durationOption);

		command.SetHandler(async (macAddress, duration) =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			var bulb = await DiscoverAndFindBulb(client, macAddress);
			if (bulb == null) return;

			await client.Lan!.SetLightPowerAsync(
				bulb,
				TimeSpan.FromSeconds(duration),
				PowerState.On,
				CancellationToken.None);

			AnsiConsole.MarkupLine($"[green]✓[/] Turned on light: {bulb.MacAddressName}");
		}, macArg, durationOption);

		return command;
	}

	private static Command CreateLanOffCommand()
	{
		var command = new Command("off", "Turn light off via LAN");

		var macArg = new Argument<string>(
			"mac-address",
			description: "MAC address of the light");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(macArg);
		command.AddOption(durationOption);

		command.SetHandler(async (macAddress, duration) =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			var bulb = await DiscoverAndFindBulb(client, macAddress);
			if (bulb == null) return;

			await client.Lan!.SetLightPowerAsync(
				bulb,
				TimeSpan.FromSeconds(duration),
				PowerState.Off,
				CancellationToken.None);

			AnsiConsole.MarkupLine($"[green]✓[/] Turned off light: {bulb.MacAddressName}");
		}, macArg, durationOption);

		return command;
	}

	private static Command CreateLanColorCommand()
	{
		var command = new Command("color", "Set light color via LAN");

		var macArg = new Argument<string>(
			"mac-address",
			description: "MAC address of the light");

		var kelvinArg = new Argument<int>(
			"kelvin",
			description: "Color temperature in Kelvin (2500-9000, e.g., 2700 for warm white)");

		var durationOption = new Option<double>(
			aliases: ["--duration", "-d"],
			getDefaultValue: () => 1.0,
			description: "Transition duration in seconds");

		command.AddArgument(macArg);
		command.AddArgument(kelvinArg);
		command.AddOption(durationOption);

		command.SetHandler(async (macAddress, kelvin, duration) =>
		{
			if (kelvin < 2500 || kelvin > 9000)
			{
				AnsiConsole.MarkupLine("[red]Kelvin must be between 2500 and 9000[/]");
				return;
			}

			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			var bulb = await DiscoverAndFindBulb(client, macAddress);
			if (bulb == null) return;

			await client.Lan!.SetColorAsync(
				bulb,
				hue: 0,
				saturation: 0,
				brightness: 65535,
				kelvin: (ushort)kelvin,
				transitionDuration: TimeSpan.FromSeconds(duration),
				CancellationToken.None);

			AnsiConsole.MarkupLine($"[green]✓[/] Set color to {kelvin}K: {bulb.MacAddressName}");
		}, macArg, kelvinArg, durationOption);

		return command;
	}

	private static Command CreateLanStateCommand()
	{
		var command = new Command("state", "Get light state via LAN");

		var macArg = new Argument<string>(
			"mac-address",
			description: "MAC address of the light");

		command.AddArgument(macArg);

		command.SetHandler(async macAddress =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			var bulb = await DiscoverAndFindBulb(client, macAddress);
			if (bulb == null) return;

			var state = await client.Lan!.GetLightStateAsync(bulb, CancellationToken.None);

			if (state == null)
			{
				AnsiConsole.MarkupLine("[yellow]Could not get light state[/]");
				return;
			}

			var table = new Table
			{
				Border = TableBorder.Rounded
			};
			table.AddColumn("Property");
			table.AddColumn("Value");

			table.AddRow("Label", state.Label);
			table.AddRow("Power", state.IsOn ? "[green]On[/]" : "[dim]Off[/]");
			table.AddRow("Hue", state.Hue.ToString());
			table.AddRow("Saturation", state.Saturation.ToString());
			table.AddRow("Brightness", state.Brightness.ToString());
			table.AddRow("Kelvin", state.Kelvin.ToString());

			AnsiConsole.Write(table);
		}, macArg);

		return command;
	}

	private static Command CreateLanRenameCommand()
	{
		var command = new Command("rename", "Rename a light via LAN");

		var macArg = new Argument<string>(
			"mac-address",
			description: "MAC address of the light");

		var nameArg = new Argument<string>(
			"new-name",
			description: "New name for the light (max 32 characters)");

		command.AddArgument(macArg);
		command.AddArgument(nameArg);

		command.SetHandler(async (macAddress, newName) =>
		{
			if (string.IsNullOrWhiteSpace(newName))
			{
				AnsiConsole.MarkupLine("[red]Name cannot be empty[/]");
				return;
			}

			if (newName.Length > 32)
			{
				AnsiConsole.MarkupLine("[red]Name must be 32 characters or less[/]");
				return;
			}

			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			var bulb = await DiscoverAndFindBulb(client, macAddress);
			if (bulb == null) return;

			// Get current name first
			var oldName = await client.Lan!.GetDeviceLabelAsync(bulb, CancellationToken.None);

			// Set new name
			await client.Lan!.SetDeviceLabelAsync(bulb, newName, CancellationToken.None);

			AnsiConsole.MarkupLine($"[green]✓[/] Renamed light from '[cyan]{oldName}[/]' to '[cyan]{newName}[/]'");
			AnsiConsole.MarkupLine($"[dim]MAC: {bulb.MacAddressName}[/]");
		}, macArg, nameArg);

		return command;
	}

	private static async Task<LightBulb?> DiscoverAndFindBulb(LifxClient client, string macAddress)
	{
		// Normalize MAC address
		macAddress = macAddress.ToUpperInvariant().Replace("-", ":").Replace(".", ":");

		AnsiConsole.Status()
			.Start("Discovering devices...", ctx =>
			{
				client.StartLan(CancellationToken.None);
				client.StartDeviceDiscovery(CancellationToken.None);
				Thread.Sleep(5000);
				client.StopDeviceDiscovery();
			});

		var bulb = client.Lan?.Devices
			.OfType<LightBulb>()
			.FirstOrDefault(d => d.MacAddressName.Equals(macAddress, StringComparison.OrdinalIgnoreCase));

		if (bulb == null)
		{
			AnsiConsole.MarkupLine($"[red]✗[/] Light not found: {macAddress}");
			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine("Available devices:");

			var devices = client.Lan?.Devices.ToList() ?? [];
			foreach (var device in devices)
			{
				AnsiConsole.MarkupLine($"  [cyan]{device.MacAddressName}[/] at {device.HostName}");
			}

			return null;
		}

		return bulb;
	}

	private static Command CreateDiscoverCommand()
	{
		var command = new Command("discover", "Discover LIFX devices on local network");

		var timeoutOption = new Option<int>(
			aliases: ["--timeout", "-t"],
			getDefaultValue: () => 5,
			description: "Discovery timeout in seconds");

		command.AddOption(timeoutOption);

		command.SetHandler(async timeout =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			AnsiConsole.Status()
				.Start("Discovering devices...", ctx =>
				{
					client.StartLan(CancellationToken.None);
					client.StartDeviceDiscovery(CancellationToken.None);

					// Wait for devices to respond
					Thread.Sleep(timeout * 1000);

					client.StopDeviceDiscovery();
				});

			var devices = client.Lan?.Devices.ToList() ?? [];

			if (devices.Count == 0)
			{
				AnsiConsole.MarkupLine("[yellow]No devices found[/]");
				AnsiConsole.WriteLine();
				AnsiConsole.MarkupLine("[dim]Make sure:[/]");
				AnsiConsole.MarkupLine("[dim]  - LIFX devices are powered on[/]");
				AnsiConsole.MarkupLine("[dim]  - Devices are on the same network[/]");
				AnsiConsole.MarkupLine("[dim]  - Firewall allows UDP port 56700[/]");
				return;
			}

			var table = new Table
			{
				Border = TableBorder.Rounded
			};
			table.AddColumn("Type");
			table.AddColumn("MAC Address");
			table.AddColumn("IP Address");
			table.AddColumn("Port");

			foreach (var device in devices)
			{
				table.AddRow(
					device.GetType().Name,
					device.MacAddressName,
					device.HostName,
					device.Port.ToString()
				);
			}

			AnsiConsole.Write(table);
			AnsiConsole.MarkupLine($"[green]✓[/] Found [cyan]{devices.Count}[/] device(s)");
		}, timeoutOption);

		return command;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List cached LAN devices from previous discovery");

		command.SetHandler(() =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });
			client.StartLan(CancellationToken.None);

			var devices = client.Lan?.Devices.ToList() ?? [];

			if (devices.Count == 0)
			{
				AnsiConsole.MarkupLine("[yellow]No cached devices found[/]");
				AnsiConsole.MarkupLine("[dim]Run 'lifx lan discover' first[/]");
				return;
			}

			var table = new Table
			{
				Border = TableBorder.Rounded
			};
			table.AddColumn("Type");
			table.AddColumn("MAC Address");
			table.AddColumn("IP Address");

			foreach (var device in devices)
			{
				table.AddRow(
					device.GetType().Name,
					device.MacAddressName,
					device.HostName
				);
			}

			AnsiConsole.Write(table);
			AnsiConsole.MarkupLine($"[dim]Total: {devices.Count} device(s)[/]");
		});

		return command;
	}
}
