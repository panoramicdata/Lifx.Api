using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class SetStateRequest
{
	[JsonPropertyName("power")]
	public PowerState Power { get; set; }

	[JsonPropertyName("color")]
	public string? Color { get; set; }

	[JsonPropertyName("brightness")]
	public double? Brightness { get; set; }

	[JsonPropertyName("duration")]
	public double? Duration { get; set; } = 1.0;

	[JsonPropertyName("infrared")]
	public double? Infrared { get; set; }

	[JsonPropertyName("fast")]
	public bool? Fast { get; set; } = false;
}
