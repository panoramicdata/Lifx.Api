using System.Text.Json.Serialization;
using Lifx.Api.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Product information for a light as returned by the LIFX API
/// </summary>
public class LightProduct
{
	[JsonPropertyName("name")]
	[JsonInclude]
	public string Name { get; private set; } = string.Empty;

	[JsonPropertyName("identifier")]
	[JsonInclude]
	public string Identifier { get; private set; } = string.Empty;

	[JsonPropertyName("company")]
	[JsonInclude]
	public string Company { get; private set; } = string.Empty;

	[JsonPropertyName("capabilities")]
	[JsonInclude]
	[JsonConverter(typeof(CapabilitiesDictionaryConverter))]
	public Dictionary<string, bool>? Capabilities { get; private set; }
}
