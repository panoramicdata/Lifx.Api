using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud;

/// <summary>
/// A color in its natural Lifx representation
/// </summary>
public class Hsbk
{
	[JsonPropertyName("hue")]
	public float? Hue { get; init; }

	[JsonPropertyName("saturation")]
	public float? Saturation { get; init; }

	[JsonPropertyName("brightness")]
	public float? Brightness { get; init; }

	[JsonPropertyName("kelvin")]
	public int? Kelvin { get; init; }

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
