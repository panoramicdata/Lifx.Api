using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Product firmware upgrade information
/// </summary>
public class ProductUpgrade
{
	[JsonPropertyName("major")]
	[JsonInclude]
	public int Major { get; private set; }

	[JsonPropertyName("minor")]
	[JsonInclude]
	public int Minor { get; private set; }

	[JsonPropertyName("features")]
	[JsonInclude]
	public ProductFeatures Features { get; private set; } = new();
}
