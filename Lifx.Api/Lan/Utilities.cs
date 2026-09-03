namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

internal static class Utilities
{
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	/// <summary>
	/// Performs RgbToHsl operation.
	/// </summary>
	public static ushort[] RgbToHsl(Color rgb)
	{
		// Which channel is the brightest is decided on the original bytes rather than on the
		// normalised doubles, so the comparison is exact and no floating point equality is involved.
		var maxByte = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
		var minByte = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));

		// normalize red, green and blue values
		var r = rgb.R / 255.0;
		var g = rgb.G / 255.0;
		var b = rgb.B / 255.0;

		var max = maxByte / 255.0;
		var min = minByte / 255.0;
		var chroma = max - min;

		// A grey (every channel equal) has no chroma and so no meaningful hue; leaving it at zero
		// also avoids the division by zero the general formulae would hit.
		var h = 0.0;
		if (maxByte != minByte)
		{
			if (maxByte == rgb.R)
			{
				h = 60 * (g - b) / chroma;
				if (rgb.G < rgb.B)
				{
					h += 360;
				}
			}
			else if (maxByte == rgb.G)
			{
				h = 60 * (b - r) / chroma + 120;
			}
			else
			{
				h = 60 * (r - g) / chroma + 240;
			}
		}

		var s = maxByte == 0 ? 0.0 : 1.0 - (min / max);
		return [
				(ushort)(h / 360 * 65535),
				(ushort)(s * 65535),
				(ushort)(max * 65535)
			];
	}
}
