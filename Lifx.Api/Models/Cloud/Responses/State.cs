using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class State
{
	[JsonPropertyName("brightness")]
	public required float Brightness { get; init; }

	[JsonPropertyName("selector")]
	public required string Selector { get; init; }

	[JsonPropertyName("color")]
	public required Hsbk? Color { get; init; }
}
