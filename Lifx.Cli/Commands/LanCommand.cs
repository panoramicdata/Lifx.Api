using System.CommandLine;
using Lifx.Api;
using Lifx.Api.Models.Cloud;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class LanCommand
{
	public static Command Create()
	{
		var command = new Command("lan", "LAN protocol operations")
		{
			CreateDiscoverCommand(),
			CreateListCommand()
		};

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
				return;
			}

			var table = new Table();
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
			AnsiConsole.MarkupLine($"[dim]Found {devices.Count} device(s)[/]");
		}, timeoutOption);

		return command;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List cached LAN devices");

		command.SetHandler(() =>
		{
			using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });
			client.StartLan(CancellationToken.None);

			var devices = client.Lan?.Devices.ToList() ?? [];

			if (devices.Count == 0)
			{
				AnsiConsole.MarkupLine("[yellow]No cached devices. Run 'dotnet lifx lan discover' first.[/]");
				return;
			}

			var table = new Table();
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
