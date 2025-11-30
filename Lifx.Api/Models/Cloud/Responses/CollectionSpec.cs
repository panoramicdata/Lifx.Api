using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

internal class CollectionSpec
{
	[JsonPropertyName("id")]
	[JsonInclude]
	public string Id { get; set; } = string.Empty;

	[JsonPropertyName("name")]
	[JsonInclude]
	public string Name { get; set; } = string.Empty;

	public override bool Equals(object? obj) => obj is CollectionSpec spec && spec.Id == Id && spec.Name == Name;

	public override int GetHashCode() => HashCode.Combine(Id, Name);
}
