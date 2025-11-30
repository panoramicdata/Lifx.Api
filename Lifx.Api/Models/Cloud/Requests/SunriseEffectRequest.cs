using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class SunriseEffectRequest
{
	[JsonPropertyName("duration")]
	public double? Duration { get; set; }

	[JsonPropertyName("power_on")]
	public bool? PowerOn { get; set; } = true;
}
