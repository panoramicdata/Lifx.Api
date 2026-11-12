using Lifx.Api;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;
using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the LanCommand type.
/// </summary>
public static partial class LanCommand
{
	/// <summary>
	/// Performs Create operation.
	/// </summary>
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

	private static Command CreateDiscoverCommand()
	{
		var command = new Command("discover", "Discover LIFX devices on local network");

		var timeoutOption = new Option<int>("--timeout", "-t")
		{
			Description = "Discovery timeout in seconds",
			DefaultValueFactory = _ => 5
		};

		command.Options.Add(timeoutOption);

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			var timeout = parseResult.GetValue(timeoutOption);

			var factory = new LifxClientFactory();
			using var client = factory.CreateLanClient();

			var devices = DiscoverDevices(client, TimeSpan.FromSeconds(timeout), cancellationToken);

			if (devices.Count == 0)
			{
				WriteNoDevicesFoundHelp();
				return;
			}

			AnsiConsole.Write(BuildDeviceTable(devices, includePort: true));
			AnsiConsole.MarkupLine($"[green]✓[/] Found [cyan]{devices.Count}[/] device(s)");
		});

		return command;
	}

	private static void WriteNoDevicesFoundHelp()
	{
		AnsiConsole.MarkupLine("[yellow]No devices found[/]");
		AnsiConsole.WriteLine();
		AnsiConsole.MarkupLine("[dim]Make sure:[/]");
		AnsiConsole.MarkupLine("[dim]  - LIFX devices are powered on[/]");
		AnsiConsole.MarkupLine("[dim]  - Devices are on the same network[/]");
		AnsiConsole.MarkupLine("[dim]  - Firewall allows UDP port 56700[/]");
	}

	/// <summary>
	/// Builds the device table. Only 'discover' shows the port column, which is the single
	/// difference between the two tables this file used to build separately.
	/// </summary>
	private static Table BuildDeviceTable(IEnumerable<Device> devices, bool includePort)
	{
		var table = new Table
		{
			Border = TableBorder.Rounded
		};
		table.AddColumn("Type");
		table.AddColumn("MAC Address");
		table.AddColumn("IP Address");
		if (includePort)
		{
			table.AddColumn("Port");
		}

		foreach (var device in devices)
		{
			var row = new List<string>
			{
				device.GetType().Name,
				device.MacAddressName,
				device.HostName
			};

			if (includePort)
			{
				row.Add(device.Port.ToString());
			}

			table.AddRow([.. row]);
		}

		return table;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List cached discovered devices");

		command.SetAction(async (_, cancellationToken) =>
		{
			var factory = new LifxClientFactory();
			using var client = factory.CreateLanClient();

			var devices = DiscoverDevices(client, DefaultDiscoveryWindow, cancellationToken);

			if (devices.Count == 0)
			{
				AnsiConsole.MarkupLine("[yellow]No devices found[/]");
				return;
			}

			AnsiConsole.Write(BuildDeviceTable(devices, includePort: false));
			AnsiConsole.MarkupLine($"[dim]Total: {devices.Count} devices[/]");
		});

		return command;
	}
}
