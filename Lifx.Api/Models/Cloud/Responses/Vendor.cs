using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Vendor type.
/// </summary>
public class Vendor
{
	/// <summary>
	/// Gets or sets VendorId.
	/// </summary>
	/// <summary>
	/// Gets or sets VendorId.
	/// </summary>
	/// <summary>
	/// Gets or sets VendorId.
	/// </summary>
	/// <summary>
	/// Gets or sets VendorId.
	/// </summary>
	[JsonPropertyName("vid")]
	[JsonInclude]
	public int VendorId { get; private set; }

	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	[JsonPropertyName("name")]
	[JsonInclude]
	public string Name { get; private set; } = string.Empty;

	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	[JsonPropertyName("defaults")]
	[JsonInclude]
	public ProductFeatures Defaults { get; private set; } = new();

	/// <summary>
	/// Gets or sets Products.
	/// </summary>
	/// <summary>
	/// Gets or sets Products.
	/// </summary>
	/// <summary>
	/// Gets or sets Products.
	/// </summary>
	/// <summary>
	/// Gets or sets Products.
	/// </summary>
	[JsonPropertyName("products")]
	[JsonInclude]
	public List<Product> Products { get; private set; } = [];
}
