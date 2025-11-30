using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class SetStatesRequest
{
	[JsonPropertyName("states")]
	public required List<StateUpdate> States { get; set; }

	[JsonPropertyName("defaults")]
	public required StateDefaults Defaults { get; set; }
}

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

public class StateDefaults
{
	[JsonPropertyName("power")]
	public PowerState? Power { get; set; }

	[JsonPropertyName("duration")]
	public double? Duration { get; set; }

	[JsonPropertyName("infrared")]
	public double? Infrared { get; set; }
}
