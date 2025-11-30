using System.CommandLine;
using Lifx.Api;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class LanCommand
{
	public static Command Create()
	{
		var command = new Command("lan", "Control LIFX lights via LAN protocol (no API token needed)")
		{
			CreateDiscoverCommand(),
			CreateListCommand()
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
			Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"  lifx lan discover --timeout 10    # Discover for 10 seconds" + Environment.NewLine +
			"  lifx lan list                     # Show discovered devices" + Environment.NewLine +
			Environment.NewLine +
			"Note: Devices must be on the same network as your computer.";

		return command;
	}

	private static Command CreateDiscoverCommand()
	{
		var command = new Command("discover", "Discover LIFX devices on local network");

		var timeoutOption = new Option<int>(
			aliases: ["--timeout", "-t"],
			getDefaultValue: () => 5,
			description: "Discovery timeout in seconds");

		command.AddOption(timeoutOption);

		command.SetHandler(async (int timeout) =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

			AnsiConsole.Status()
				.Start("Discovering devices...", ctx =>
				{
					client.StartLan(CancellationToken.None);

					// Wait for devices
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

			var table = new Table();
			table.Border = TableBorder.Rounded;
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
			AnsiConsole.MarkupLine($"[green]?[/] Found [cyan]{devices.Count}[/] device(s)");
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

			var table = new Table();
			table.Border = TableBorder.Rounded;
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
