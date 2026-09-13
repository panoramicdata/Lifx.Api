using AwesomeAssertions;
using Lifx.Api.Lan;
using Lifx.Api.Models.Lan;
using System.Data;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Tests for utility methods (RGB to HSL conversion, epoch time, color helpers)
/// </summary>
/// <summary>
/// Represents the UtilitiesTests type.
/// </summary>
[Collection("Unit Tests")]
public class UtilitiesTests
{
	#region RGB to HSL Conversion Tests

	/// <summary>
	/// Reference colours and the HSL triple each must convert to. Hue is given in degrees and
	/// scaled the way the conversion does, so each expectation reads as a colour wheel value
	/// rather than a magic number.
	/// </summary>
	public static TheoryData<byte, byte, byte, double, ushort, ushort> RgbToHslCases => new()
	{
		// r,   g,    b,   hue degrees, saturation, value
		{ 255, 0, 0, 0, 65535, 65535 },       // red
		{ 0, 255, 0, 120, 65535, 65535 },     // green
		{ 0, 0, 255, 240, 65535, 65535 },     // blue
		{ 0, 255, 255, 180, 65535, 65535 },   // cyan
		{ 255, 0, 255, 300, 65535, 65535 },   // magenta
		{ 255, 255, 0, 60, 65535, 65535 },    // yellow
		{ 255, 255, 255, 0, 0, 65535 },       // white - hue undefined, algorithm returns 0
		{ 0, 0, 0, 0, 0, 0 },                 // black - hue and saturation undefined
		{ 128, 128, 128, 0, 0, 32896 }        // grey - hue and saturation undefined
	};

	/// <summary>
	/// Performs RgbToHsl_Should_Convert_Correctly operation.
	/// </summary>
	[Theory]
	[MemberData(nameof(RgbToHslCases))]
	public void RgbToHsl_Should_Convert_Correctly(
		byte r,
		byte g,
		byte b,
		double expectedHueDegrees,
		ushort expectedSaturation,
		ushort expectedValue)
	{
		// Arrange
		var color = new Color { R = r, G = g, B = b };

		// Act
		var hsl = Utilities.RgbToHsl(color);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		hsl[0].Should().Be((ushort)(expectedHueDegrees / 360.0 * 65535)); // Hue
		hsl[1].Should().Be(expectedSaturation); // Saturation
		hsl[2].Should().Be(expectedValue); // Lightness/Value
	}

	#endregion

	#region Epoch Tests

	/// <summary>
	/// Performs Epoch_Should_Be_Unix_Epoch operation.
	/// </summary>
	[Fact]
	public void Epoch_Should_Be_Unix_Epoch()
	{
		// Assert
		Utilities.Epoch.Should().Be(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
	}

	/// <summary>
	/// Performs Epoch_Should_Be_UTC operation.
	/// </summary>
	[Fact]
	public void Epoch_Should_Be_UTC()
	{
		// Assert
		Utilities.Epoch.Kind.Should().Be(DateTimeKind.Utc);
	}

	/// <summary>
	/// Performs Epoch_Should_Have_Zero_Time_Components operation.
	/// </summary>
	[Fact]
	public void Epoch_Should_Have_Zero_Time_Components()
	{
		// Assert
		Utilities.Epoch.Hour.Should().Be(0);
		Utilities.Epoch.Minute.Should().Be(0);
		Utilities.Epoch.Second.Should().Be(0);
		Utilities.Epoch.Millisecond.Should().Be(0);
	}

	#endregion

	#region LifxColor BuildRGB Tests

	/// <summary>
	/// Performs BuildRGB_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void BuildRGB_Should_Format_Correctly()
	{
		// Act
		var color = LifxColor.BuildRGB(255, 0, 0);

		// Assert
		color.Should().Be("rgb:255,0,0");
	}

	/// <summary>
	/// Performs BuildRGB_Should_Handle_All_Zeros operation.
	/// </summary>
	[Fact]
	public void BuildRGB_Should_Handle_All_Zeros()
	{
		// Act
		var color = LifxColor.BuildRGB(0, 0, 0);

		// Assert
		color.Should().Be("rgb:0,0,0");
	}

	/// <summary>
	/// Performs BuildRGB_Should_Handle_All_Max_Values operation.
	/// </summary>
	[Fact]
	public void BuildRGB_Should_Handle_All_Max_Values()
	{
		// Act
		var color = LifxColor.BuildRGB(255, 255, 255);

		// Assert
		color.Should().Be("rgb:255,255,255");
	}


	/// <summary>
	/// Channel values outside 0-255, one per channel and at each end of the range.
	/// </summary>
	public static TheoryData<int, int, int> OutOfRangeRgbCases => new()
	{
		{ 256, 0, 0 },  // red too high
		{ -1, 0, 0 },   // red too low
		{ 0, 256, 0 },  // green too high
		{ 0, 0, 256 }   // blue too high
	};

	/// <summary>
	/// Performs BuildRGB_Should_Throw_On_Out_Of_Range_Channel operation.
	/// </summary>
	[Theory]
	[MemberData(nameof(OutOfRangeRgbCases))]
	public void BuildRGB_Should_Throw_On_Out_Of_Range_Channel(int red, int green, int blue)
		// Act & Assert
		=> ((Func<string>)(() => LifxColor.BuildRGB(red, green, blue)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();

	#endregion

	#region LifxColor BuildHSBK Tests

	/// <summary>
	/// Performs BuildHSBK_Should_Format_All_Components operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_Should_Format_All_Components()
	{
		// Act
		var color = LifxColor.BuildHSBK(120, 0.5, 0.8, 3500);

		// Assert
		color.Should().Contain("hue:120");
		color.Should().Contain("saturation:0.5");
		color.Should().Contain("brightness:0.8");
		color.Should().Contain("kelvin:3500");
	}

	/// <summary>
	/// Performs BuildHSBK_Should_Allow_Null_Hue operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_Should_Allow_Null_Hue()
	{
		// Act
		var color = LifxColor.BuildHSBK(null, 0.5, 0.8, 3500);

		// Assert
		color.Should().NotContain("hue:");
		color.Should().Contain("saturation:0.5");
		color.Should().Contain("brightness:0.8");
		color.Should().Contain("kelvin:3500");
	}

	/// <summary>
	/// Performs BuildHSBK_Should_Throw_When_All_Null operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_Should_Throw_When_All_Null()
	{
		// Act & Assert
		((Func<string>)(() => LifxColor.BuildHSBK(null, null, null, null)))
			.Should()
			.ThrowExactly<ArgumentException>();
	}

	/// <summary>
	/// One component out of range per case, at each end of its own range, with the other three
	/// left at values the builder accepts.
	/// </summary>
	public static TheoryData<double?, double?, double?, int?> OutOfRangeHsbkCases => new()
	{
		{ -1, 0.5, 0.5, 3500 },    // hue too low
		{ 361, 0.5, 0.5, 3500 },   // hue too high
		{ 120, -0.1, 0.5, 3500 },  // saturation too low
		{ 120, 1.1, 0.5, 3500 },   // saturation too high
		{ 120, 0.5, -0.1, 3500 },  // brightness too low
		{ 120, 0.5, 1.1, 3500 },   // brightness too high
		{ 120, 0.5, 0.5, 1499 },   // kelvin below minimum
		{ 120, 0.5, 0.5, 9001 }    // kelvin above maximum
	};

	/// <summary>
	/// Performs BuildHSBK_Should_Validate_Component_Ranges operation.
	/// </summary>
	[Theory]
	[MemberData(nameof(OutOfRangeHsbkCases))]
	public void BuildHSBK_Should_Validate_Component_Ranges(
		double? hue,
		double? saturation,
		double? brightness,
		int? kelvin)
		// Act & Assert
		=> ((Func<string>)(() => LifxColor.BuildHSBK(hue, saturation, brightness, kelvin)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();

	/// <summary>
	/// Performs BuildHSBK_Should_Allow_Min_Kelvin operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_Should_Allow_Min_Kelvin()
	{
		// Act
		var color = LifxColor.BuildHSBK(null, null, 0.5, 1500);

		// Assert
		color.Should().Contain("kelvin:1500");
	}

	/// <summary>
	/// Performs BuildHSBK_Should_Allow_Max_Kelvin operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_Should_Allow_Max_Kelvin()
	{
		// Act
		var color = LifxColor.BuildHSBK(null, null, 0.5, 9000);

		// Assert
		color.Should().Contain("kelvin:9000");
	}

	#endregion

	#region LifxColor Named Colors Tests

	/// <summary>
	/// Performs NamedColors_Should_Contain_Expected_Values operation.
	/// </summary>
	[Fact]
	public void NamedColors_Should_Contain_Expected_Values()
	{
		// Assert
		LifxColor.NamedColors.Should().Contain("white");
		LifxColor.NamedColors.Should().Contain("red");
		LifxColor.NamedColors.Should().Contain("orange");
		LifxColor.NamedColors.Should().Contain("yellow");
		LifxColor.NamedColors.Should().Contain("green");
		LifxColor.NamedColors.Should().Contain("cyan");
		LifxColor.NamedColors.Should().Contain("blue");
		LifxColor.NamedColors.Should().Contain("purple");
		LifxColor.NamedColors.Should().Contain("pink");
	}

	/// <summary>
	/// Performs NamedColors_Should_Have_Nine_Colors operation.
	/// </summary>
	[Fact]
	public void NamedColors_Should_Have_Nine_Colors()
	{
		// Assert
		LifxColor.NamedColors.Should().HaveCount(9);
	}

	/// <summary>
	/// Performs DefaultWhite_Should_Be_White_String operation.
	/// </summary>
	[Fact]
	public void DefaultWhite_Should_Be_White_String()
	{
		// Assert
		LifxColor.DefaultWhite.Should().Be("white");
	}

	/// <summary>
	/// Performs Color_Constants_Should_Have_Correct_Values operation.
	/// </summary>
	[Fact]
	public void Color_Constants_Should_Have_Correct_Values()
	{
		// Assert
		LifxColor.TemperatureMin.Should().Be(1500);
		LifxColor.TemperatureMax.Should().Be(9000);
		LifxColor.TemperatureDefault.Should().Be(3500);
	}

	#endregion
}
