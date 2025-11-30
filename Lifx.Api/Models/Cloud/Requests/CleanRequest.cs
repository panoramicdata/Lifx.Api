using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class CleanRequest
{
	[JsonPropertyName("stop")]
	public bool? Stop { get; set; } = false;
	[JsonPropertyName("duration")]
	public double? Duration { get; set; }
}
