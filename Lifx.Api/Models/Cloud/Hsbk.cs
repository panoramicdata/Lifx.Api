using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud;

/// <summary>
/// Represents the Hsbk type.
/// </summary>
public class Hsbk
{
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	[JsonPropertyName("hue")]
	public float? Hue { get; set; }

	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	[JsonPropertyName("saturation")]
	public float? Saturation { get; set; }

	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	[JsonPropertyName("brightness")]
	public float? Brightness { get; set; }

	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	[JsonPropertyName("kelvin")]
	public int? Kelvin { get; set; }

	/// <summary>
	/// Performs ToString operation.
	/// </summary>
	public override string ToString()
	{
		StringBuilder sb = new();
		if (Hue is not null)
		{
			sb.AppendFormat("hue:{0} ", Math.Min(Math.Max(0, Hue.Value), 360));
		}

		if (Saturation is not null)
		{
			sb.AppendFormat("saturation:{0} ", Math.Min(Math.Max(0, Saturation.Value), 1));
		}

		if (Brightness is not null)
		{
			sb.AppendFormat("brightness:{0} ", Math.Min(Math.Max(0, Brightness.Value), 1));
		}

		if (Kelvin is not null && (Saturation ?? 0) < 0.001)
		{
			sb.AppendFormat("kelvin:{0} ", Math.Min(Math.Max(LifxColor.TemperatureMin, Kelvin.Value), LifxColor.TemperatureMax));
		}

		if (sb.Length > 0)
		{
			sb.Remove(sb.Length - 1, 1);
		}

		return sb.ToString();
	}
}
