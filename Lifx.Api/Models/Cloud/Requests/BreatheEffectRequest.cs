using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class BreatheEffectRequest
{
	[JsonPropertyName("color")]
	public required string Color { get; set; }
	[JsonPropertyName("from_color")]
	public string? FromColor { get; set; }
	[JsonPropertyName("period")]
	public double? Period { get; set; } = 1.0;
	[JsonPropertyName("cycles")]
	public double? Cycles { get; set; } = 1.0;
	[JsonPropertyName("peak")]
	public double? Peak { get; set; } = .5;
	[JsonPropertyName("persist")]
	public bool? Persist { get; set; } = false;
	[JsonPropertyName("power_on")]
	public bool? PowerOn { get; set; } = true;
}
