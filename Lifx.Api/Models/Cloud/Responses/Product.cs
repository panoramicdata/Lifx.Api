using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// LIFX Product
/// </summary>
public class Product
{
	[JsonPropertyName("pid")]
	[JsonInclude]
	public int ProductId { get; private set; }

	[JsonPropertyName("name")]
	[JsonInclude]
	public string Name { get; private set; } = string.Empty;

	[JsonPropertyName("features")]
	[JsonInclude]
	public ProductFeatures Features { get; private set; } = new();

	[JsonPropertyName("upgrades")]
	[JsonInclude]
	public List<ProductUpgrade> Upgrades { get; private set; } = [];
}
