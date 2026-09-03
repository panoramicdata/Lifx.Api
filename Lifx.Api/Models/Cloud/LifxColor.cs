using System.Data;

namespace Lifx.Api.Models.Cloud;

/// <summary>
/// Represents the LifxColor type.
/// </summary>
public abstract class LifxColor
{
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static int TemperatureMin => 1500;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static int TemperatureMax => 9000;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static int TemperatureDefault => 3500;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string DefaultWhite = "white";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Red = "red";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Orange = "orange";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Yellow = "yellow";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Cyan = "cyan";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Green = "green";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Blue = "blue";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Purple = "purple";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly string Pink = "pink";

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static string White { get; } = BuildHSBK(null, null, 1f, TemperatureDefault);


	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public static readonly IEnumerable<string> NamedColors =
	[
		DefaultWhite, Red, Orange, Yellow, Cyan, Green, Blue, Purple, Pink
	];

	/// <summary>
	/// Performs BuildHSBK operation.
	/// </summary>
	public static string BuildHSBK(double? hue, double? saturation, double? brightness, int? kelvin)
	{
		if (hue is null && saturation is null && brightness is null && kelvin is null)
		{
			throw new ArgumentException("HSBKColor requires at least one non-null component");
		}

		StringBuilder colorString = new();
		AppendHueSaturationBrightness(colorString, hue, saturation, brightness);
		AppendComponent(colorString, " kelvin", kelvin, TemperatureMin, TemperatureMax, $"{TemperatureMin}-{TemperatureMax}");
		return colorString.ToString();
	}

	/// <summary>
	/// Performs BuildHSB operation.
	/// </summary>
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
		AppendHueSaturationBrightness(colorString, hue, saturation, brightness);
		return colorString.ToString();
	}

	private static void AppendHueSaturationBrightness(
		StringBuilder colorString,
		double? hue,
		double? saturation,
		double? brightness)
	{
		AppendComponent(colorString, "hue", hue, 0, 360, "0-360");
		AppendComponent(colorString, " saturation", saturation, 0.0, 1.0, "0.0-1.0");
		AppendComponent(colorString, " brightness", brightness, 0.0, 1.0, "0.0-1.0");
	}

	/// <summary>
	/// Appends one optional component to the selector, rejecting it when it falls outside its range.
	/// The element name carries its own separator, so an omitted component leaves no gap behind.
	/// </summary>
	private static void AppendComponent(
		StringBuilder colorString,
		string element,
		double? value,
		double min,
		double max,
		string validRange)
	{
		if (value is null)
		{
			return;
		}

		if (!IsBetween(value.Value, min, max))
		{
			var name = char.ToUpperInvariant(element.Trim()[0]) + element.Trim()[1..];
			throw new InvalidConstraintException($"Value for {name} is invalid, valid range[{validRange}]");
		}

		colorString.Append(FormatString(element, value));
	}

	/// <summary>
	/// Performs BuildRGB operation.
	/// </summary>
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
