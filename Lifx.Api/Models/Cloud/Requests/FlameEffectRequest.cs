using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class FlameEffectRequest
{
	[JsonPropertyName("period")]
	public double? Period { get; set; } = 5;
	[JsonPropertyName("duration")]
	public double? Duration { get; set; }
	[JsonPropertyName("power_on")]
	public bool? PowerOn { get; set; } = true;
}
