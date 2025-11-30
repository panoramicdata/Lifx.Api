using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

internal class CollectionSpec
{
	[JsonPropertyName("id")]
	[JsonInclude]
	public string id { get; set; } = string.Empty;

	[JsonPropertyName("name")]
	[JsonInclude]
	public string name { get; set; } = string.Empty;

	public override bool Equals(object? obj) => obj is CollectionSpec spec && spec.id == id && spec.name == name;

	public override int GetHashCode() => HashCode.Combine(id, name);
}
