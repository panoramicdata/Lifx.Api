using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class StateUpdate
{
	[JsonPropertyName("selector")]
	public required string Selector { get; set; }

	[JsonPropertyName("power")]
	public PowerState? Power { get; set; }

	[JsonPropertyName("color")]
	public required string Color { get; set; }

	[JsonPropertyName("brightness")]
	public double? Brightness { get; set; }

	[JsonPropertyName("duration")]
	public double? Duration { get; set; }

	[JsonPropertyName("infrared")]
	public double? Infrared { get; set; }
}
