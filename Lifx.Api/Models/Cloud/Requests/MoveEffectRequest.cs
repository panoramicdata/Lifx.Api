using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

public class MoveEffectRequest
{
	[JsonPropertyName("direction")]
	public string Direction { get; set; } = "forward";
	[JsonPropertyName("period")]
	public double? Period { get; set; } = 1;
	[JsonPropertyName("cycles")]
	public double? Cycles { get; set; }
	[JsonPropertyName("power_on")]
	public bool? PowerOn { get; set; } = true;
}
