using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Cli.Handlers;

/// <summary>
/// Encapsulates the business logic for product catalog operations.
/// </summary>
public static class ProductsHandler
{
	/// <summary>
	/// Filters vendors by name (case-insensitive).
	/// </summary>
	public static List<Vendor> FilterByVendor(IReadOnlyList<Vendor> vendors, string? vendorFilter)
	{
		if (string.IsNullOrEmpty(vendorFilter))
		{
			return [.. vendors];
		}

		return [.. vendors.Where(v => v.Name.Contains(vendorFilter, StringComparison.OrdinalIgnoreCase))];
	}

	/// <summary>
	/// Filters products by capability flags.
	/// </summary>
	public static IEnumerable<Product> FilterProducts(IEnumerable<Product> products, bool colorOnly, bool multizoneOnly)
	{
		if (colorOnly)
		{
			products = products.Where(p => p.Features.Color);
		}

		if (multizoneOnly)
		{
			products = products.Where(p => p.Features.Multizone);
		}

		return products;
	}
}
