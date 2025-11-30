using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class CycleRequest
{
	[JsonPropertyName("states")]
	public required List<SetStateRequest> States { get; set; }

	[JsonPropertyName("defaults")]
	public required SetStateRequest Defaults { get; set; }

	[JsonPropertyName("direction")]
	public string Direction { get; set; } = "forward";
}
