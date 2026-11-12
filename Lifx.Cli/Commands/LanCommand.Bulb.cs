using Lifx.Api;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;
using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

public static partial class LanCommand
{
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
		=> TargetCommandBuilder.Build(
			name,
			description,
			macArgument,
			extraArguments,
			options,
			precondition.Invoke,
			async (parseResult, macAddress, cancellationToken) =>
			{
				var factory = new LifxClientFactory();
				using var client = factory.CreateLanClient();

				var bulb = await DiscoverAndFindBulb(client, macAddress, cancellationToken);
				if (bulb is not null)
				{
					await action(client, bulb, parseResult, cancellationToken);
				}
			});

	private static Argument<string> CreateMacArgument(string description)
		=> new("mac-address") { Description = description };

	private static Option<double> CreateDurationOption()
		=> TargetCommandBuilder.CreateDurationOption();

	private static Command CreateLanPowerCommand(
		string name,
		string description,
		string macDescription,
		PowerState powerState)
	{
		var durationOption = CreateDurationOption();

		return CreateBulbCommand(
			name,
			description,
			CreateMacArgument(macDescription),
			[],
			[durationOption],
			(client, bulb, parseResult, cancellationToken) => SetLanPowerAsync(
				client,
				bulb,
				powerState,
				parseResult.GetValue(durationOption),
				cancellationToken));
	}

	private static Command CreateLanOnCommand()
		=> CreateLanPowerCommand(
			"on",
			"Turn light on via LAN",
			"MAC address of the light (e.g., D0:73:D5:12:34:56)",
			PowerState.On);

	private static Command CreateLanOffCommand()
		=> CreateLanPowerCommand(
			"off",
			"Turn light off via LAN",
			"MAC address of the light",
			PowerState.Off);

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
		=> TryValidate<ArgumentOutOfRangeException>(() => LanHandler.ValidateKelvin(kelvin));

	private static bool TryValidate<TException>(Action validate)
		where TException : ArgumentException
	{
		try
		{
			validate();
			return true;
		}
		catch (TException ex)
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
		=> TryValidate<ArgumentException>(() => LanHandler.ValidateLightName(newName));

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

	/// <summary>
	/// How long to listen for device replies when the caller has no say in it.
	/// </summary>
	private static readonly TimeSpan DefaultDiscoveryWindow = TimeSpan.FromSeconds(5);

	/// <summary>
	/// Runs LAN discovery for the given window and returns whatever answered.
	/// </summary>
	/// <remarks>
	/// Discovery is always start, wait, stop against the same client; only the wait differs. Three
	/// copies of this block were what drove the duplication in this file.
	/// </remarks>
	private static List<Device> DiscoverDevices(
		ILifxClient client,
		TimeSpan window,
		CancellationToken cancellationToken)
	{
		AnsiConsole.Status()
			.Start("Discovering devices...", ctx =>
			{
				client.StartLan(cancellationToken);
				client.StartDeviceDiscovery(cancellationToken);

				// Wait for devices to respond
				Thread.Sleep(window);

				client.StopDeviceDiscovery();
			});

		return client.Lan?.Devices.ToList() ?? [];
	}

	private static async Task<LightBulb?> DiscoverAndFindBulb(
		ILifxClient client,
		string macAddress,
		CancellationToken cancellationToken)
	{
		// Normalize MAC address
		macAddress = LanHandler.NormalizeMacAddress(macAddress);

		var devices = DiscoverDevices(client, DefaultDiscoveryWindow, cancellationToken);

		var bulb = devices
			.OfType<LightBulb>()
			.FirstOrDefault(d => d.MacAddressName.Equals(macAddress, StringComparison.OrdinalIgnoreCase));

		if (bulb == null)
		{
			AnsiConsole.MarkupLine($"[red]✗[/] Light not found: {macAddress}");
			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine("Available devices:");

			foreach (var device in devices)
			{
				AnsiConsole.MarkupLine($"  [cyan]{device.MacAddressName}[/] at {device.HostName}");
			}

			return null;
		}

		return bulb;
	}
}
