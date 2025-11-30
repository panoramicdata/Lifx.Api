using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class EffectsOffRequest
{
	[JsonPropertyName("power_off")]
	public bool? PowerOff { get; set; } = false;
}
