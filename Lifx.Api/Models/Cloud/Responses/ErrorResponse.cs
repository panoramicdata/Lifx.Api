using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class ErrorResponse : ApiResponse
{
	[JsonPropertyName("error")]
	public string? Error { get; init; }

	[JsonPropertyName("errors")]
	public List<Error>? Errors { get; init; }
}
