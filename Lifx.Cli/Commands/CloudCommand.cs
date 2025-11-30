using System.CommandLine;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class CloudCommand
{
	public static Command Create()
	{
		var command = new Command("cloud", "Control LIFX lights via Cloud API (requires API token)")
		{
			LightsCommand.Create(),
			EffectsCommand.Create(),
			ScenesCommand.Create()
		};

		command.Description =
			"Control LIFX lights via Cloud API (requires API token)" + Environment.NewLine +
			Environment.NewLine +
			"The Cloud API allows you to control your LIFX lights from anywhere with internet." + Environment.NewLine +
			"Requires an API token from https://cloud.lifx.com/settings" + Environment.NewLine +
			Environment.NewLine +
			"Subcommands:" + Environment.NewLine +
			"  lights    - Control light power, color, and brightness" + Environment.NewLine +
			"  effects   - Run visual effects (breathe, pulse, etc.)" + Environment.NewLine +
			"  scenes    - List and activate scenes" + Environment.NewLine +
			Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"  lifx cloud lights list             # List all lights" + Environment.NewLine +
			"  lifx cloud lights on all           # Turn on all lights" + Environment.NewLine +
			"  lifx cloud lights color all blue   # Set color" + Environment.NewLine +
			"  lifx cloud effects breathe all     # Start breathe effect" + Environment.NewLine +
			"  lifx cloud scenes activate \"Movie Time\"  # Activate scene" + Environment.NewLine +
			Environment.NewLine +
			"Setup:" + Environment.NewLine +
			"  1. Get token: https://cloud.lifx.com/settings" + Environment.NewLine +
			"  2. Store token: lifx key set <token>" + Environment.NewLine +
			"  3. Run commands: lifx cloud lights on all";

		return command;
	}
}
