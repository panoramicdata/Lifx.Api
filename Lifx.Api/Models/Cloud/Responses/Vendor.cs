using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// LIFX Vendor (manufacturer)
/// </summary>
public class Vendor
{
	[JsonPropertyName("vid")]
	[JsonInclude]
	public int VendorId { get; private set; }

	[JsonPropertyName("name")]
	[JsonInclude]
	public string Name { get; private set; } = string.Empty;

	[JsonPropertyName("defaults")]
	[JsonInclude]
	public ProductFeatures Defaults { get; private set; } = new();

	[JsonPropertyName("products")]
	[JsonInclude]
	public List<Product> Products { get; private set; } = [];
}
