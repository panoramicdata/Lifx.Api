using System.Text.Json;
using AwesomeAssertions;
using Lifx.Api.Models.Cloud.Responses;
using Lifx.Cli.Handlers;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for ProductsHandler filtering.
/// </summary>
[Collection("Unit Tests")]
public class ProductsHandlerTests
{
	/// <summary>
	/// Tests FilterByVendor with null filter returns all vendors.
	/// </summary>
	[Fact]
	public void FilterByVendor_NullFilter_ReturnsAll()
	{
		var vendors = CreateVendors();

		var result = ProductsHandler.FilterByVendor(vendors, null);

		result.Should().HaveCount(2);
	}

	/// <summary>
	/// Tests FilterByVendor with empty filter returns all vendors.
	/// </summary>
	[Fact]
	public void FilterByVendor_EmptyFilter_ReturnsAll()
	{
		var vendors = CreateVendors();

		var result = ProductsHandler.FilterByVendor(vendors, "");

		result.Should().HaveCount(2);
	}

	/// <summary>
	/// Tests FilterByVendor with matching filter returns filtered results.
	/// </summary>
	[Fact]
	public void FilterByVendor_MatchingFilter_ReturnsFiltered()
	{
		var vendors = CreateVendors();

		var result = ProductsHandler.FilterByVendor(vendors, "LIFX");

		result.Should().HaveCount(1);
		result[0].Name.Should().Be("LIFX");
	}

	/// <summary>
	/// Tests FilterByVendor is case insensitive.
	/// </summary>
	[Fact]
	public void FilterByVendor_CaseInsensitive()
	{
		var vendors = CreateVendors();

		var result = ProductsHandler.FilterByVendor(vendors, "lifx");

		result.Should().HaveCount(1);
	}

	/// <summary>
	/// Tests FilterByVendor with no match returns empty.
	/// </summary>
	[Fact]
	public void FilterByVendor_NoMatch_ReturnsEmpty()
	{
		var vendors = CreateVendors();

		var result = ProductsHandler.FilterByVendor(vendors, "Philips");

		result.Should().BeEmpty();
	}

	/// <summary>
	/// Tests FilterProducts with no filters returns all products.
	/// </summary>
	[Fact]
	public void FilterProducts_NoFilters_ReturnsAll()
	{
		var products = CreateProducts();

		var result = ProductsHandler.FilterProducts(products, colorOnly: false, multizoneOnly: false).ToList();

		result.Should().HaveCount(3);
	}

	/// <summary>
	/// Tests FilterProducts with color only filter.
	/// </summary>
	[Fact]
	public void FilterProducts_ColorOnly_FiltersCorrectly()
	{
		var products = CreateProducts();

		var result = ProductsHandler.FilterProducts(products, colorOnly: true, multizoneOnly: false).ToList();

		result.Should().HaveCount(2);
		result.Should().OnlyContain(p => p.Features.Color);
	}

	/// <summary>
	/// Tests FilterProducts with multizone only filter.
	/// </summary>
	[Fact]
	public void FilterProducts_MultizoneOnly_FiltersCorrectly()
	{
		var products = CreateProducts();

		var result = ProductsHandler.FilterProducts(products, colorOnly: false, multizoneOnly: true).ToList();

		result.Should().HaveCount(1);
		result[0].Features.Multizone.Should().BeTrue();
	}

	/// <summary>
	/// Tests FilterProducts with both filters applied.
	/// </summary>
	[Fact]
	public void FilterProducts_BothFilters_FiltersCorrectly()
	{
		var products = CreateProducts();

		var result = ProductsHandler.FilterProducts(products, colorOnly: true, multizoneOnly: true).ToList();

		result.Should().HaveCount(1);
		result[0].Features.Color.Should().BeTrue();
		result[0].Features.Multizone.Should().BeTrue();
	}

	private static List<Vendor> CreateVendors() =>
		JsonSerializer.Deserialize<List<Vendor>>("""
			[
				{ "vid": 1, "name": "LIFX", "products": [] },
				{ "vid": 2, "name": "Other", "products": [] }
			]
			""")!;

	private static List<Product> CreateProducts() =>
		JsonSerializer.Deserialize<List<Product>>("""
			[
				{ "pid": 1, "name": "LIFX A19", "features": { "color": true, "multizone": false } },
				{ "pid": 2, "name": "LIFX Z Strip", "features": { "color": true, "multizone": true } },
				{ "pid": 3, "name": "LIFX White", "features": { "color": false, "multizone": false } }
			]
			""")!;
}
