using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

internal class CollectionSpec
{
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	[JsonPropertyName("id")]
	[JsonInclude]
	public string Id { get; set; } = string.Empty;

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
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Performs Equals operation.
	/// </summary>
	public override bool Equals(object? obj) => obj is CollectionSpec spec && spec.Id == Id && spec.Name == Name;

	/// <summary>
	/// Performs GetHashCode operation.
	/// </summary>
	public override int GetHashCode() => HashCode.Combine(Id, Name);
}
