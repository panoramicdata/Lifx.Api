using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Product type.
/// </summary>
public class Product
{
	/// <summary>
	/// Gets or sets ProductId.
	/// </summary>
	/// <summary>
	/// Gets or sets ProductId.
	/// </summary>
	/// <summary>
	/// Gets or sets ProductId.
	/// </summary>
	/// <summary>
	/// Gets or sets ProductId.
	/// </summary>
	[JsonPropertyName("pid")]
	[JsonInclude]
	public int ProductId { get; private set; }

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
	/// Gets or sets Features.
	/// </summary>
	/// <summary>
	/// Gets or sets Features.
	/// </summary>
	/// <summary>
	/// Gets or sets Features.
	/// </summary>
	/// <summary>
	/// Gets or sets Features.
	/// </summary>
	[JsonPropertyName("features")]
	[JsonInclude]
	public ProductFeatures Features { get; private set; } = new();

	/// <summary>
	/// Gets or sets Upgrades.
	/// </summary>
	/// <summary>
	/// Gets or sets Upgrades.
	/// </summary>
	/// <summary>
	/// Gets or sets Upgrades.
	/// </summary>
	/// <summary>
	/// Gets or sets Upgrades.
	/// </summary>
	[JsonPropertyName("upgrades")]
	[JsonInclude]
	public List<ProductUpgrade> Upgrades { get; private set; } = [];
}
