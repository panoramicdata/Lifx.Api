using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the MoveEffectRequest type.
/// </summary>
public class MoveEffectRequest
{
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	[JsonPropertyName("direction")]
	public string Direction { get; set; } = "forward";
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	[JsonPropertyName("period")]
	public double? Period { get; set; } = 1;
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	[JsonPropertyName("cycles")]
	public double? Cycles { get; set; }
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	[JsonPropertyName("power_on")]
	public bool? PowerOn { get; set; } = true;
}
