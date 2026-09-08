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
public static class LanCommand
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

	/// <summary>
	/// Runs one operation against a bulb that has already been discovered.
	/// </summary>
	private delegate Task BulbAction(
		ILifxClient client,
		LightBulb bulb,
		ParseResult parseResult,
		CancellationToken cancellationToken);

	/// <summary>
	/// Rejects bad input before discovery starts. Returns false once a message has been shown.
	/// </summary>
	private delegate bool BulbPrecondition(ParseResult parseResult);

	/// <summary>
	/// Builds one subcommand that acts on a single light identified by MAC address, with nothing
	/// to check before discovery.
	/// </summary>
	private static Command CreateBulbCommand(
		string name,
		string description,
		Argument<string> macArgument,
		Argument[] extraArguments,
		Option[] options,
		BulbAction action)
		=> CreateBulbCommand(
			name,
			description,
			macArgument,
			extraArguments,
			options,
			_ => true,
			action);

	/// <summary>
	/// Builds one subcommand that acts on a single light identified by MAC address.
	/// </summary>
	/// <remarks>
	/// Each of these commands took a MAC address, opened a LAN client, discovered the bulb and gave
	/// up quietly if it was not found. Only what happens after that differs, so that is all a
	/// caller supplies here.
	/// </remarks>
	private static Command CreateBulbCommand(
		string name,
		string description,
		Argument<string> macArgument,
		Argument[] extraArguments,
		Option[] options,
		BulbPrecondition precondition,
		BulbAction action)
	{
		var command = new Command(name, description);

		command.Arguments.Add(macArgument);
		foreach (var argument in extraArguments)
		{
			command.Arguments.Add(argument);
		}

		foreach (var option in options)
		{
			command.Options.Add(option);
		}

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			if (!precondition(parseResult))
			{
				return;
			}

			var macAddress = parseResult.GetValue(macArgument)!;

			var factory = new LifxClientFactory();
			using var client = factory.CreateLanClient();

			var bulb = await DiscoverAndFindBulb(client, macAddress, cancellationToken);
			if (bulb is null)
			{
				return;
			}

			await action(client, bulb, parseResult, cancellationToken);
		});

		return command;
	}

	private static Argument<string> CreateMacArgument(string description)
		=> new("mac-address") { Description = description };

	private static Option<double> CreateDurationOption()
		=> new("--duration", "-d")
		{
			Description = "Transition duration in seconds",
			DefaultValueFactory = _ => 1.0
		};

	private static Command CreateLanOnCommand()
	{
		var durationOption = CreateDurationOption();

		return CreateBulbCommand(
			"on",
			"Turn light on via LAN",
			CreateMacArgument("MAC address of the light (e.g., D0:73:D5:12:34:56)"),
			[],
			[durationOption],
			(client, bulb, parseResult, cancellationToken) => SetLanPowerAsync(
				client,
				bulb,
				PowerState.On,
				parseResult.GetValue(durationOption),
				cancellationToken));
	}

	private static Command CreateLanOffCommand()
	{
		var durationOption = CreateDurationOption();

		return CreateBulbCommand(
			"off",
			"Turn light off via LAN",
			CreateMacArgument("MAC address of the light"),
			[],
			[durationOption],
			(client, bulb, parseResult, cancellationToken) => SetLanPowerAsync(
				client,
				bulb,
				PowerState.Off,
				parseResult.GetValue(durationOption),
				cancellationToken));
	}

	private static async Task SetLanPowerAsync(
		ILifxClient client,
		LightBulb bulb,
		PowerState powerState,
		double duration,
		CancellationToken cancellationToken)
	{
		await client.Lan!.SetLightPowerAsync(
			bulb,
			TimeSpan.FromSeconds(duration),
			powerState,
			cancellationToken);

		var verb = powerState == PowerState.On ? "on" : "off";
		AnsiConsole.MarkupLine($"[green]✓[/] Turned {verb} light: {bulb.MacAddressName}");
	}

	/// <summary>
	/// Reports an out-of-range colour temperature to the console and returns false, so the caller
	/// can bail out without repeating the try/catch at every call site.
	/// </summary>
	private static bool TryValidateKelvin(int kelvin)
	{
		try
		{
			LanHandler.ValidateKelvin(kelvin);
			return true;
		}
		catch (ArgumentOutOfRangeException ex)
		{
			AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
			return false;
		}
	}

	private static Command CreateLanColorCommand()
	{
		var kelvinArgument = new Argument<int>("kelvin")
		{
			Description = "Color temperature in Kelvin (2500-9000, e.g., 2700 for warm white)"
		};
		var durationOption = CreateDurationOption();

		return CreateBulbCommand(
			"color",
			"Set light color via LAN",
			CreateMacArgument("MAC address of the light"),
			[kelvinArgument],
			[durationOption],
			parseResult => TryValidateKelvin(parseResult.GetValue(kelvinArgument)),
			(client, bulb, parseResult, cancellationToken) => SetLanColorAsync(
				client,
				bulb,
				parseResult.GetValue(kelvinArgument),
				parseResult.GetValue(durationOption),
				cancellationToken));
	}

	private static async Task SetLanColorAsync(
		ILifxClient client,
		LightBulb bulb,
		int kelvin,
		double duration,
		CancellationToken cancellationToken)
	{
		await client.Lan!.SetColorAsync(
			bulb,
			hue: 0,
			saturation: 0,
			brightness: 65535,
			kelvin: (ushort)kelvin,
			transitionDuration: TimeSpan.FromSeconds(duration),
			cancellationToken);

		AnsiConsole.MarkupLine($"[green]✓[/] Set color to {kelvin}K: {bulb.MacAddressName}");
	}

	private static Command CreateLanStateCommand()
		=> CreateBulbCommand(
			"state",
			"Get light state via LAN",
			CreateMacArgument("MAC address of the light"),
			[],
			[],
			(client, bulb, _, cancellationToken) => ShowLanStateAsync(client, bulb, cancellationToken));

	private static async Task ShowLanStateAsync(
		ILifxClient client,
		LightBulb bulb,
		CancellationToken cancellationToken)
	{
		var state = await client.Lan!.GetLightStateAsync(bulb, cancellationToken);

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
	}

	private static Command CreateLanRenameCommand()
	{
		var nameArgument = new Argument<string>("new-name")
		{
			Description = "New name for the light (max 32 characters)"
		};

		return CreateBulbCommand(
			"rename",
			"Rename a light via LAN",
			CreateMacArgument("MAC address of the light"),
			[nameArgument],
			[],
			parseResult => TryValidateLightName(parseResult.GetValue(nameArgument)!),
			(client, bulb, parseResult, cancellationToken) => RenameLanLightAsync(
				client,
				bulb,
				parseResult.GetValue(nameArgument)!,
				cancellationToken));
	}

	/// <summary>
	/// Reports an unusable light name to the console and returns false.
	/// </summary>
	private static bool TryValidateLightName(string newName)
	{
		try
		{
			LanHandler.ValidateLightName(newName);
			return true;
		}
		catch (ArgumentException ex)
		{
			AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
			return false;
		}
	}

	private static async Task RenameLanLightAsync(
		ILifxClient client,
		LightBulb bulb,
		string newName,
		CancellationToken cancellationToken)
	{
		// Get current name first
		var oldName = await client.Lan!.GetDeviceLabelAsync(bulb, cancellationToken);

		// Set new name
		await client.Lan!.SetDeviceLabelAsync(bulb, newName, cancellationToken);

		AnsiConsole.MarkupLine($"[green]✓[/] Renamed light from '[cyan]{oldName}[/]' to '[cyan]{newName}[/]'");
		AnsiConsole.MarkupLine($"[dim]MAC: {bulb.MacAddressName}[/]");
	}

	private static async Task<LightBulb?> DiscoverAndFindBulb(
		ILifxClient client,
		string macAddress,
		CancellationToken cancellationToken)
	{
		// Normalize MAC address
		macAddress = LanHandler.NormalizeMacAddress(macAddress);

		AnsiConsole.Status()
			.Start("Discovering devices...", ctx =>
			{
				client.StartLan(cancellationToken);
				client.StartDeviceDiscovery(cancellationToken);
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

			AnsiConsole.Status()
				.Start("Discovering devices...", ctx =>
				{
					client.StartLan(cancellationToken);
					client.StartDeviceDiscovery(cancellationToken);

					// Wait for devices to respond
					Thread.Sleep(timeout * 1000);

					client.StopDeviceDiscovery();
				});

			var devices = client.Lan?.Devices.ToList() ?? [];

			if (devices.Count == 0)
			{
				WriteNoDevicesFoundHelp();
				return;
			}

			AnsiConsole.Write(BuildDeviceTable(devices));
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

	private static Table BuildDeviceTable(IEnumerable<Device> devices)
	{
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

		return table;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List cached discovered devices");

		command.SetAction(async (_, cancellationToken) =>
		{
			var factory = new LifxClientFactory();
			using var client = factory.CreateLanClient();

			AnsiConsole.Status()
				.Start("Discovering devices...", ctx =>
				{
					client.StartLan(cancellationToken);
					client.StartDeviceDiscovery(cancellationToken);
					Thread.Sleep(5000);
					client.StopDeviceDiscovery();
				});

			var devices = client.Lan?.Devices.ToList() ?? [];

			if (devices.Count == 0)
			{
				AnsiConsole.MarkupLine("[yellow]No devices found[/]");
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
			AnsiConsole.MarkupLine($"[dim]Total: {devices.Count} devices[/]");
		});

		return command;
	}
}
