using Lifx.Api;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

public static class ProductsCommand
{
	public static Command Create()
	{
		var command = new Command("products", "View LIFX product catalog")
		{
			CreateListCommand()
		};

		return command;
	}

	private static Command CreateListCommand()
	{
		var command = new Command("list", "List all LIFX products");

		var vendorOption = new Option<string?>(
			aliases: ["--vendor", "-v"],
			description: "Filter by vendor name (default: LIFX)");

		var colorOnlyOption = new Option<bool>(
			aliases: ["--color-only"],
			getDefaultValue: () => false,
			description: "Show only color-capable products");

		var multizoneOnlyOption = new Option<bool>(
			aliases: ["--multizone-only"],
			getDefaultValue: () => false,
			description: "Show only multizone products");

		command.AddOption(vendorOption);
		command.AddOption(colorOnlyOption);
		command.AddOption(multizoneOnlyOption);

		command.SetHandler(async (verbose, vendorFilter, colorOnly, multizoneOnly) =>
		{
			// Products API doesn't require a token
			using var client = new LifxClient(new LifxClientOptions());

			var vendors = await client.Products.GetAllAsync(CancellationToken.None);

			// Filter by vendor if specified
			if (!string.IsNullOrEmpty(vendorFilter))
			{
				vendors = [.. vendors.Where(v => v.Name.Contains(vendorFilter, StringComparison.OrdinalIgnoreCase))];
			}

			var table = new Table
			{
				Border = TableBorder.Rounded
			};
			table.AddColumn("PID");
			table.AddColumn("Product Name");
			table.AddColumn("Color");
			table.AddColumn("Temp Range");

			if (verbose)
			{
				table.AddColumn("HEV");
				table.AddColumn("Infrared");
				table.AddColumn("Multizone");
				table.AddColumn("Matrix");
			}

			int totalCount = 0;

			foreach (var vendor in vendors)
			{
				var products = vendor.Products.AsEnumerable();

				// Apply filters
				if (colorOnly)
				{
					products = products.Where(p => p.Features.Color);
				}

				if (multizoneOnly)
				{
					products = products.Where(p => p.Features.Multizone);
				}

				foreach (var product in products)
				{
					var tempRange = product.Features.TemperatureRange != null && product.Features.TemperatureRange.Length == 2
						? $"{product.Features.TemperatureRange[0]}-{product.Features.TemperatureRange[1]}K"
						: "N/A";

					var row = new List<string>
					{
						product.ProductId.ToString(),
						product.Name,
						product.Features.Color ? "[green]Yes[/]" : "[dim]No[/]",
						tempRange
					};

					if (verbose)
					{
						row.Add(product.Features.Hev ? "[green]Yes[/]" : "[dim]No[/]");
						row.Add(product.Features.Infrared ? "[green]Yes[/]" : "[dim]No[/]");
						row.Add(product.Features.Multizone ? "[green]Yes[/]" : "[dim]No[/]");
						row.Add(product.Features.Matrix ? "[green]Yes[/]" : "[dim]No[/]");
					}

					table.AddRow(row.ToArray());
					totalCount++;
				}
			}

			AnsiConsole.Write(table);
			AnsiConsole.MarkupLine($"[dim]Total: {totalCount} products[/]");

			if (vendorFilter != null)
			{
				AnsiConsole.MarkupLine($"[dim]Filtered by vendor: {vendorFilter}[/]");
			}

			if (colorOnly)
			{
				AnsiConsole.MarkupLine("[dim]Showing only color-capable products[/]");
			}

			if (multizoneOnly)
			{
				AnsiConsole.MarkupLine("[dim]Showing only multizone products[/]");
			}
		},
		new VerboseBinder(),
		vendorOption,
		colorOnlyOption,
		multizoneOnlyOption);

		return command;
	}
}
