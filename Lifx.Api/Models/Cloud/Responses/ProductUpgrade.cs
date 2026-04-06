using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the ProductUpgrade type.
/// </summary>
public class ProductUpgrade
{
	/// <summary>
	/// Gets or sets Major.
	/// </summary>
	/// <summary>
	/// Gets or sets Major.
	/// </summary>
	/// <summary>
	/// Gets or sets Major.
	/// </summary>
	/// <summary>
	/// Gets or sets Major.
	/// </summary>
	[JsonPropertyName("major")]
	[JsonInclude]
	public int Major { get; private set; }

	/// <summary>
	/// Gets or sets Minor.
	/// </summary>
	/// <summary>
	/// Gets or sets Minor.
	/// </summary>
	/// <summary>
	/// Gets or sets Minor.
	/// </summary>
	/// <summary>
	/// Gets or sets Minor.
	/// </summary>
	[JsonPropertyName("minor")]
	[JsonInclude]
	public int Minor { get; private set; }

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
}
