using Lifx.Cli.Handlers;
using Spectre.Console;
using System.CommandLine;

namespace Lifx.Cli.Commands;

/// <summary>
/// Represents the ProductsCommand type.
/// </summary>
public static class ProductsCommand
{
	/// <summary>
	/// Performs Create operation.
	/// </summary>
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

		var vendorOption = new Option<string?>("--vendor", "-v")
		{
			Description = "Filter by vendor name (default: LIFX)"
		};

		var colorOnlyOption = new Option<bool>("--color-only")
		{
			Description = "Show only color-capable products",
			DefaultValueFactory = _ => false
		};

		var multizoneOnlyOption = new Option<bool>("--multizone-only")
		{
			Description = "Show only multizone products",
			DefaultValueFactory = _ => false
		};

		command.Options.Add(vendorOption);
		command.Options.Add(colorOnlyOption);
		command.Options.Add(multizoneOnlyOption);

		command.SetAction(async (parseResult, cancellationToken) =>
		{
			var verbose = parseResult.GetValue(GlobalOptions.Verbose);
			var vendorFilter = parseResult.GetValue(vendorOption);
			var colorOnly = parseResult.GetValue(colorOnlyOption);
			var multizoneOnly = parseResult.GetValue(multizoneOnlyOption);

			// Products API doesn't require a token
			var factory = new LifxClientFactory();
			using var client = factory.CreateAnonymousClient();

			var vendors = await client.Products.GetAllAsync(cancellationToken);

			// Filter by vendor if specified
			var filteredVendors = ProductsHandler.FilterByVendor(vendors, vendorFilter);

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

			var totalCount = 0;

			foreach (var vendor in filteredVendors)
			{
				var products = ProductsHandler.FilterProducts(vendor.Products, colorOnly, multizoneOnly);

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
		});

		return command;
	}
}
