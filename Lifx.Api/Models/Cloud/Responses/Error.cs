using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Error type.
/// </summary>
public class Error
{
	/// <summary>
	/// Gets or sets Field.
	/// </summary>
	/// <summary>
	/// Gets or sets Field.
	/// </summary>
	/// <summary>
	/// Gets or sets Field.
	/// </summary>
	/// <summary>
	/// Gets or sets Field.
	/// </summary>
	[JsonPropertyName("field")]
	public required string Field { get; init; }

	/// <summary>
	/// Gets or sets Message.
	/// </summary>
	/// <summary>
	/// Gets or sets Message.
	/// </summary>
	/// <summary>
	/// Gets or sets Message.
	/// </summary>
	/// <summary>
	/// Gets or sets Message.
	/// </summary>
	[JsonPropertyName("message")]
	public required string[] Message { get; init; }
}
