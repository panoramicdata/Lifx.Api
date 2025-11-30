using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class Error
{
	[JsonPropertyName("field")]
	public required string Field { get; init; }

	[JsonPropertyName("message")]
	public required string[] Message { get; init; }
}
