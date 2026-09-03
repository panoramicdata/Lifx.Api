using Lifx.Api.Models.Cloud.Responses;
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
			var filteredVendors = ProductsHandler.FilterByVendor(vendors, vendorFilter);

			var table = CreateProductsTable(verbose);
			var totalCount = AddProductRows(table, filteredVendors, colorOnly, multizoneOnly, verbose);

			AnsiConsole.Write(table);
			AnsiConsole.MarkupLine($"[dim]Total: {totalCount} products[/]");
			WriteActiveFilters(vendorFilter, colorOnly, multizoneOnly);
		});

		return command;
	}

	private static Table CreateProductsTable(bool verbose)
	{
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

		return table;
	}

	/// <summary>
	/// Adds a row per matching product and returns how many were written.
	/// </summary>
	private static int AddProductRows(
		Table table,
		IEnumerable<Vendor> vendors,
		bool colorOnly,
		bool multizoneOnly,
		bool verbose)
	{
		var totalCount = 0;

		foreach (var vendor in vendors)
		{
			foreach (var product in ProductsHandler.FilterProducts(vendor.Products, colorOnly, multizoneOnly))
			{
				table.AddRow(BuildProductRow(product, verbose));
				totalCount++;
			}
		}

		return totalCount;
	}

	private static string[] BuildProductRow(Product product, bool verbose)
	{
		var temperatureRange = product.Features.TemperatureRange is { Length: 2 } range
			? $"{range[0]}-{range[1]}K"
			: "N/A";

		var row = new List<string>
		{
			product.ProductId.ToString(),
			product.Name,
			Flag(product.Features.Color),
			temperatureRange
		};

		if (verbose)
		{
			row.Add(Flag(product.Features.Hev));
			row.Add(Flag(product.Features.Infrared));
			row.Add(Flag(product.Features.Multizone));
			row.Add(Flag(product.Features.Matrix));
		}

		return [.. row];
	}

	private static string Flag(bool value) => value ? "[green]Yes[/]" : "[dim]No[/]";

	private static void WriteActiveFilters(string? vendorFilter, bool colorOnly, bool multizoneOnly)
	{
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
	}
}
