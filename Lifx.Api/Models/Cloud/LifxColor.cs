using System.Data;

namespace Lifx.Api.Models.Cloud;

/// <summary>
/// The color of light is best represented in terms of hue, saturation,
/// kelvin, and brightness components. However, other means of expressing
/// colors are available
/// </summary>
public abstract class LifxColor
{
	/// <summary>
	/// Color temperature should be at least 1500K
	/// </summary>
	public const int TemperatureMin = 1500;
	/// <summary>
	/// Color temperature should be at most 9000K
	/// </summary>
	public const int TemperatureMax = 9000;
	/// <summary>
	/// A normal white color temperature, this corresponds to the DefaultWhite color.
	/// </summary>
	public const int TemperatureDefault = 3500;
	/// <summary>
	/// Sets saturation to 0
	/// </summary>
	public static readonly string DefaultWhite = "white";
	/// <summary>
	/// Sets hue to 0
	/// </summary>
	public static readonly string Red = "red";
	/// <summary>
	/// Sets hue to 34
	/// </summary>
	public static readonly string Orange = "orange";
	/// <summary>
	/// Sets hue to 60
	/// </summary>
	public static readonly string Yellow = "yellow";
	/// <summary>
	/// Sets hue to 180
	/// </summary>
	public static readonly string Cyan = "cyan";
	/// <summary>
	/// Sets hue to 120
	/// </summary>
	public static readonly string Green = "green";
	/// <summary>
	/// Sets hue to 250
	/// </summary>
	public static readonly string Blue = "blue";
	/// <summary>
	/// Sets hue to 280
	/// </summary>
	public static readonly string Purple = "purple";
	/// <summary>
	/// Sets hue to 325
	/// </summary>
	public static readonly string Pink = "pink";

	/// <summary>
	/// A configurable white Light Color
	/// </summary>
	public static string White = BuildHSBK(null, null, 1f, TemperatureDefault);


	public static readonly IEnumerable<string> NamedColors =
	[
		DefaultWhite, Red, Orange, Yellow, Cyan, Green, Blue, Purple, Pink
	];

	public static string BuildHSBK(double? hue, double? saturation, double? brightness, int? kelvin)
	{
		if (hue is null && saturation is null && brightness is null && kelvin is null)
		{
			throw new ArgumentException("HSBKColor requires at least one non-null component");
		}

		StringBuilder colorString = new();

		//check hue
		if (hue is not null)
		{
			if (!IsBetween(hue.Value, 0, 360))
			{
				throw new InvalidConstraintException("Value for Hue is invalid, valid range[0-360]");
			}

			colorString.Append(FormatString("hue", hue));
		}

		//check saturation
		if (saturation is not null)
		{
			if (!IsBetween(saturation.Value, 0.0, 1.0))
			{
				throw new InvalidConstraintException("Value for Saturation is invalid, valid range[0.0-1.0]");
			}

			colorString.Append(FormatString(" saturation", saturation));
		}

		//check brightness
		if (brightness is not null)
		{
			if (!IsBetween(brightness.Value, 0.0, 1.0))
			{
				throw new InvalidConstraintException("Value for Brightness is invalid, valid range[0.0-1.0]");
			}

			colorString.Append(FormatString(" brightness", brightness.ToString()));
		}

		//check kelvin
		if (kelvin is not null)
		{
			if (!IsBetween(kelvin.Value, TemperatureMin, TemperatureMax))
			{
				throw new InvalidConstraintException("Value for Kelvin is invalid, valid range[1500-9000]");
			}

			colorString.Append(FormatString(" kelvin", kelvin));
		}

		return colorString.ToString();
	}

	public static string BuildHSB(
		double? hue,
		double? saturation,
		double? brightness)
	{
		if (hue is null && saturation is null && brightness is null)
		{
			throw new ArgumentException("HSBColor requires at least one non-null component");
		}

		StringBuilder colorString = new();

		//check hue
		if (hue is not null)
		{
			if (!IsBetween(hue.Value, 0, 360))
			{
				throw new InvalidConstraintException("Value for Hue is invalid, valid range[0-360]");
			}

			colorString.Append(FormatString("hue", hue.ToString()));
		}

		//check saturation
		if (saturation is not null)
		{
			if (!IsBetween(saturation.Value, 0.0, 1.0))
			{
				throw new InvalidConstraintException("Value for Saturation is invalid, valid range[0.0-1.0]");
			}

			colorString.Append(FormatString(" saturation", saturation.ToString()));
		}

		//check brightness
		if (brightness is not null)
		{
			if (!IsBetween(brightness.Value, 0.0, 1.0))
			{
				throw new InvalidConstraintException("Value for Brightness is invalid, valid range[0.0-1.0]");
			}

			colorString.Append(FormatString(" brightness", brightness.ToString()));
		}

		return colorString.ToString();
	}

	public static string BuildRGB(int red, int green, int blue)
	{
		//check red
		if (!IsBetween(Convert.ToDouble(red), 0, 255))
		{
			throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
		}

		//check green
		if (!IsBetween(Convert.ToDouble(green), 0, 255))
		{
			throw new InvalidConstraintException("Value for Green is invalid, valid range[0-255]");
		}

		//check blue
		if (!IsBetween(Convert.ToDouble(blue), 0, 255))
		{
			throw new InvalidConstraintException("Value for Blue is invalid, valid range[0-255]");
		}

		return $"rgb:{red},{green},{blue}";
	}

	private static string FormatString<T>(string element, T? value)
	{
		if (value is not null)
		{
			return $"{element}:{value}";
		}
		else
		{
			return string.Empty;
		}
	}

	private static bool IsBetween(double item, double min, double max) => item >= min && item <= max;
}
