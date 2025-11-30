using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class ColorResult
{
	[JsonPropertyName("hue")]
	public int? Hue { get; set; }

	[JsonPropertyName("saturation")]
	public float? Saturation { get; set; }

	[JsonPropertyName("brightness")]
	public float? Brightness { get; set; }

	[JsonPropertyName("kelvin")]
	public float? Kelvin { get; set; }
}
