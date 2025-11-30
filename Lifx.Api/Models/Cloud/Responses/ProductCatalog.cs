using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// LIFX Product catalog containing all vendors and their products
/// </summary>
public class ProductCatalog
{
	[JsonPropertyName("vendors")]
	[JsonInclude]
	public List<Vendor> Vendors { get; private set; } = [];
}
