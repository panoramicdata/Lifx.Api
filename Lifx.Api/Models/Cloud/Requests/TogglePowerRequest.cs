using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class TogglePowerRequest
{
	[JsonPropertyName("duration")]
	public double? Duration { get; set; } = 1.0;
}
