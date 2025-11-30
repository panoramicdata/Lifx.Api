using System.CommandLine;
using Spectre.Console;

namespace Lifx.Cli.Commands;

public static class CloudCommand
{
	public static Command Create()
	{
		var command = new Command("cloud", "Control LIFX lights via Cloud API (requires API token)")
		{
			CreateKeyCommand(),
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
			"  key       - Manage API token (set/show/delete)" + Environment.NewLine +
			"  lights    - Control light power, color, and brightness" + Environment.NewLine +
			"  effects   - Run visual effects (breathe, pulse, etc.)" + Environment.NewLine +
			"  scenes    - List and activate scenes" + Environment.NewLine +
			Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"  lifx cloud key set <token>         # Store API token (REQUIRED)" + Environment.NewLine +
			"  lifx cloud lights list             # List all lights" + Environment.NewLine +
			"  lifx cloud lights on all           # Turn on all lights" + Environment.NewLine +
			"  lifx cloud lights color all blue   # Set color" + Environment.NewLine +
			"  lifx cloud effects breathe all     # Start breathe effect" + Environment.NewLine +
			"  lifx cloud scenes activate \"Movie Time\"  # Activate scene" + Environment.NewLine +
			Environment.NewLine +
			"Setup:" + Environment.NewLine +
			"  1. Get token: https://cloud.lifx.com/settings" + Environment.NewLine +
			"  2. Store token: lifx cloud key set <token>" + Environment.NewLine +
			"  3. Run commands: lifx cloud lights on all";

		return command;
	}

	private static Command CreateKeyCommand()
	{
		var command = new Command("key", "Manage LIFX Cloud API token")
		{
			CreateSetKeyCommand(),
			CreateShowKeyCommand(),
			CreateDeleteKeyCommand()
		};

		command.Description = 
			"Manage LIFX Cloud API token (securely stored in Windows Credential Manager)" + Environment.NewLine +
			Environment.NewLine +
			"The API token is required for all Cloud API features." + Environment.NewLine +
			"Token is stored encrypted in Windows Credential Manager for security." + Environment.NewLine +
			Environment.NewLine +
			"Get your API token:" + Environment.NewLine +
			"  1. Visit https://cloud.lifx.com/settings" + Environment.NewLine +
			"  2. Log in to your LIFX account" + Environment.NewLine +
			"  3. Click 'Generate New Token'" + Environment.NewLine +
			"  4. Copy the token and store it using: lifx cloud key set <token>" + Environment.NewLine +
			Environment.NewLine +
			"Security Note:" + Environment.NewLine +
			"  - Never share your API token" + Environment.NewLine +
			"  - Token is stored encrypted and never displayed in full" + Environment.NewLine +
			"  - Clear your command history after setting token";

		return command;
	}

	private static Command CreateSetKeyCommand()
	{
		var command = new Command("set", "Store API token securely in Windows Credential Manager");

		var tokenArg = new Argument<string>(
			"token",
			description: "API token from https://cloud.lifx.com/settings");

		command.AddArgument(tokenArg);

		command.SetHandler((string token) =>
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				AnsiConsole.MarkupLine("[red]? API token cannot be empty[/]");
				return;
			}

			var success = SecureCredentialManager.StoreApiToken(token);

			if (success)
			{
				AnsiConsole.MarkupLine("[green]? API token stored securely[/]");
				AnsiConsole.MarkupLine("[dim]Location: Windows Credential Manager (encrypted)[/]");
				AnsiConsole.WriteLine();
				AnsiConsole.MarkupLine("[yellow]? Security Reminder:[/]");
				AnsiConsole.MarkupLine("[dim]  - Clear your command history to remove the token[/]");
				AnsiConsole.MarkupLine("[dim]  - Never share screenshots containing your token[/]");
				AnsiConsole.WriteLine();
				AnsiConsole.MarkupLine("You can now use LIFX Cloud commands:");
				AnsiConsole.MarkupLine("  [cyan]lifx cloud lights list[/]");
				AnsiConsole.MarkupLine("  [cyan]lifx cloud lights on all[/]");
				AnsiConsole.MarkupLine("  [cyan]lifx cloud effects breathe all[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]? Failed to store API token[/]");
				AnsiConsole.MarkupLine("[dim]Check Windows Credential Manager permissions[/]");
			}
		}, tokenArg);

		return command;
	}

	private static Command CreateShowKeyCommand()
	{
		var command = new Command("show", "Check if API token is configured (does not display token)");

		command.SetHandler(() =>
		{
			var hasToken = SecureCredentialManager.HasStoredToken();

			if (hasToken)
			{
				var token = SecureCredentialManager.GetApiToken();
				var masked = token != null && token.Length > 8
					? $"{token[..4]}...{token[^4..]}"
					: "***";

				AnsiConsole.MarkupLine($"[green]? API token is configured[/]");
				AnsiConsole.MarkupLine($"[dim]Token (masked): {masked}[/]");
				AnsiConsole.MarkupLine($"[dim]Storage: Windows Credential Manager[/]");
				AnsiConsole.WriteLine();
				AnsiConsole.MarkupLine("[dim]Note: Full token is never displayed for security[/]");
			}
			else
			{
				var envToken = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
				if (!string.IsNullOrWhiteSpace(envToken))
				{
					AnsiConsole.MarkupLine("[yellow]? API token from environment variable[/]");
					AnsiConsole.MarkupLine("[dim]LIFX_API_TOKEN is set[/]");
					AnsiConsole.MarkupLine("[dim]Tip: Use 'lifx cloud key set' for secure storage[/]");
				}
				else
				{
					AnsiConsole.MarkupLine("[red]? No API token configured[/]");
					AnsiConsole.WriteLine();
					AnsiConsole.MarkupLine("To get an API token:");
					AnsiConsole.MarkupLine("  1. Visit [link]https://cloud.lifx.com/settings[/]");
					AnsiConsole.MarkupLine("  2. Log in and click 'Generate New Token'");
					AnsiConsole.MarkupLine("  3. Run: [cyan]lifx cloud key set <token>[/]");
					AnsiConsole.WriteLine();
					AnsiConsole.MarkupLine("[dim]Note: LAN features work without an API token[/]");
				}
			}
		});

		return command;
	}

	private static Command CreateDeleteKeyCommand()
	{
		var command = new Command("delete", "Remove stored API token from Windows Credential Manager");

		command.SetHandler(() =>
		{
			if (!SecureCredentialManager.HasStoredToken())
			{
				AnsiConsole.MarkupLine("[yellow]? No stored API token found[/]");
				return;
			}

			var success = SecureCredentialManager.DeleteApiToken();

			if (success)
			{
				AnsiConsole.MarkupLine("[green]? API token removed[/]");
				AnsiConsole.MarkupLine("[dim]Removed from Windows Credential Manager[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]? Failed to delete API token[/]");
			}
		});

		return command;
	}
}
