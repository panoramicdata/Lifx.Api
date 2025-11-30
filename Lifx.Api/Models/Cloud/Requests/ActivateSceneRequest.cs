using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class ActivateSceneRequest
{
	[JsonPropertyName("duration")]
	public double? Duration { get; set; } = 1.0;

	[JsonPropertyName("ignore")]
	public List<string> Ignore { get; set; } = [];

	[JsonPropertyName("overrides")]
	public SetStateRequest Overrides { get; set; } = new();

	[JsonPropertyName("fast")]
	public bool? Fast { get; set; }
}
