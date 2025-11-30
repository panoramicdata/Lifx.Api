using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class StateDeltaRequest
{
	[JsonPropertyName("power")]
	public string? Power { get; set; }
	[JsonPropertyName("hue")]
	public double? Hue { get; set; }
	[JsonPropertyName("saturation")]
	public double? Saturation { get; set; }
	[JsonPropertyName("kelvin")]
	public double? Kelvin { get; set; }
	[JsonPropertyName("brightness")]
	public double? Brightness { get; set; }
	[JsonPropertyName("duration")]
	public double? Duration { get; set; } = 1.0;
	[JsonPropertyName("infrared")]
	public double? Infrared { get; set; }
}
