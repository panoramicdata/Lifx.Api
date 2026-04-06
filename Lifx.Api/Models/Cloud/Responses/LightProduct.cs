using System.Text.Json.Serialization;
using Lifx.Api.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the LightProduct type.
/// </summary>
public class LightProduct
{
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
	/// Gets or sets Identifier.
	/// </summary>
	/// <summary>
	/// Gets or sets Identifier.
	/// </summary>
	/// <summary>
	/// Gets or sets Identifier.
	/// </summary>
	/// <summary>
	/// Gets or sets Identifier.
	/// </summary>
	[JsonPropertyName("identifier")]
	[JsonInclude]
	public string Identifier { get; private set; } = string.Empty;

	/// <summary>
	/// Gets or sets Company.
	/// </summary>
	/// <summary>
	/// Gets or sets Company.
	/// </summary>
	/// <summary>
	/// Gets or sets Company.
	/// </summary>
	/// <summary>
	/// Gets or sets Company.
	/// </summary>
	[JsonPropertyName("company")]
	[JsonInclude]
	public string Company { get; private set; } = string.Empty;

	/// <summary>
	/// Gets or sets Capabilities.
	/// </summary>
	/// <summary>
	/// Gets or sets Capabilities.
	/// </summary>
	/// <summary>
	/// Gets or sets Capabilities.
	/// </summary>
	/// <summary>
	/// Gets or sets Capabilities.
	/// </summary>
	[JsonPropertyName("capabilities")]
	[JsonInclude]
	[JsonConverter(typeof(CapabilitiesDictionaryConverter))]
	public Dictionary<string, bool>? Capabilities { get; private set; }
}
