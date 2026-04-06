using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the ProductCatalog type.
/// </summary>
public class ProductCatalog
{
	/// <summary>
	/// Gets or sets Vendors.
	/// </summary>
	/// <summary>
	/// Gets or sets Vendors.
	/// </summary>
	/// <summary>
	/// Gets or sets Vendors.
	/// </summary>
	/// <summary>
	/// Gets or sets Vendors.
	/// </summary>
	[JsonPropertyName("vendors")]
	[JsonInclude]
	public List<Vendor> Vendors { get; private set; } = [];
}
