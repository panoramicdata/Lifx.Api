using AwesomeAssertions;
using Lifx.Api.Lan;
using Lifx.Api.Models.Lan;
using System.Data;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Tests for utility methods (RGB to HSL conversion, epoch time, color helpers)
/// </summary>
[Collection("Unit Tests")]
public class UtilitiesTests
{
	#region RGB to HSL Conversion Tests

	[Fact]
	public void RgbToHsl_Should_Convert_Red_Correctly()
	{
		// Arrange
		var red = new Color { R = 255, G = 0, B = 0 };

		// Act
		var hsl = Utilities.RgbToHsl(red);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		// Red should have hue of 0 degrees
		hsl[0].Should().Be(0); // Hue
		hsl[1].Should().Be(65535); // Saturation (full)
		hsl[2].Should().Be(65535); // Lightness/Value (full)
	}

	[Fact]
	public void RgbToHsl_Should_Convert_Green_Correctly()
	{
		// Arrange
		var green = new Color { R = 0, G = 255, B = 0 };

		// Act
		var hsl = Utilities.RgbToHsl(green);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		// Green should have hue of 120 degrees
		hsl[0].Should().Be((ushort)(120.0 / 360.0 * 65535)); // Hue ~21845
		hsl[1].Should().Be(65535); // Saturation (full)
		hsl[2].Should().Be(65535); // Lightness/Value (full)
	}

	[Fact]
	public void RgbToHsl_Should_Convert_Blue_Correctly()
	{
		// Arrange
		var blue = new Color { R = 0, G = 0, B = 255 };

		// Act
		var hsl = Utilities.RgbToHsl(blue);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		// Blue should have hue of 240 degrees
		hsl[0].Should().Be((ushort)(240.0 / 360.0 * 65535)); // Hue ~43690
		hsl[1].Should().Be(65535); // Saturation (full)
		hsl[2].Should().Be(65535); // Lightness/Value (full)
	}

	[Fact]
	public void RgbToHsl_Should_Convert_White_Correctly()
	{
		// Arrange
		var white = new Color { R = 255, G = 255, B = 255 };

		// Act
		var hsl = Utilities.RgbToHsl(white);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		hsl[0].Should().Be(0); // Hue (undefined for white, but algorithm returns 0)
		hsl[1].Should().Be(0); // Saturation (none)
		hsl[2].Should().Be(65535); // Lightness/Value (full)
	}

	[Fact]
	public void RgbToHsl_Should_Convert_Black_Correctly()
	{
		// Arrange
		var black = new Color { R = 0, G = 0, B = 0 };

		// Act
		var hsl = Utilities.RgbToHsl(black);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		hsl[0].Should().Be(0); // Hue (undefined for black)
		hsl[1].Should().Be(0); // Saturation (none)
		hsl[2].Should().Be(0); // Lightness/Value (none)
	}

	[Fact]
	public void RgbToHsl_Should_Convert_Gray_Correctly()
	{
		// Arrange
		var gray = new Color { R = 128, G = 128, B = 128 };

		// Act
		var hsl = Utilities.RgbToHsl(gray);

		// Assert
		hsl.Should().NotBeNull();
		hsl.Should().HaveCount(3);
		hsl[0].Should().Be(0); // Hue (undefined for gray)
		hsl[1].Should().Be(0); // Saturation (none)
		hsl[2].Should().Be(32896); // Lightness/Value (~50%)
	}

	[Fact]
	public void RgbToHsl_Should_Handle_Cyan()
	{
		// Arrange
		var cyan = new Color { R = 0, G = 255, B = 255 };

		// Act
		var hsl = Utilities.RgbToHsl(cyan);

		// Assert
		hsl.Should().NotBeNull();
		hsl[0].Should().Be((ushort)(180.0 / 360.0 * 65535)); // Hue 180 degrees
		hsl[1].Should().Be(65535); // Full saturation
		hsl[2].Should().Be(65535); // Full value
	}

	[Fact]
	public void RgbToHsl_Should_Handle_Magenta()
	{
		// Arrange
		var magenta = new Color { R = 255, G = 0, B = 255 };

		// Act
		var hsl = Utilities.RgbToHsl(magenta);

		// Assert
		hsl.Should().NotBeNull();
		hsl[0].Should().Be((ushort)(300.0 / 360.0 * 65535)); // Hue 300 degrees
		hsl[1].Should().Be(65535); // Full saturation
		hsl[2].Should().Be(65535); // Full value
	}

	[Fact]
	public void RgbToHsl_Should_Handle_Yellow()
	{
		// Arrange
		var yellow = new Color { R = 255, G = 255, B = 0 };

		// Act
		var hsl = Utilities.RgbToHsl(yellow);

		// Assert
		hsl.Should().NotBeNull();
		hsl[0].Should().Be((ushort)(60.0 / 360.0 * 65535)); // Hue 60 degrees
		hsl[1].Should().Be(65535); // Full saturation
		hsl[2].Should().Be(65535); // Full value
	}

	#endregion

	#region Epoch Tests

	[Fact]
	public void Epoch_Should_Be_Unix_Epoch()
	{
		// Assert
		Utilities.Epoch.Should().Be(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
	}

	[Fact]
	public void Epoch_Should_Be_UTC()
	{
		// Assert
		Utilities.Epoch.Kind.Should().Be(DateTimeKind.Utc);
	}

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

	[Fact]
	public void BuildRGB_Should_Format_Correctly()
	{
		// Act
		var color = LifxColor.BuildRGB(255, 0, 0);

		// Assert
		color.Should().Be("rgb:255,0,0");
	}

	[Fact]
	public void BuildRGB_Should_Handle_All_Zeros()
	{
		// Act
		var color = LifxColor.BuildRGB(0, 0, 0);

		// Assert
		color.Should().Be("rgb:0,0,0");
	}

	[Fact]
	public void BuildRGB_Should_Handle_All_Max_Values()
	{
		// Act
		var color = LifxColor.BuildRGB(255, 255, 255);

		// Assert
		color.Should().Be("rgb:255,255,255");
	}

	[Fact]
	public void BuildRGB_Should_Throw_On_Red_Too_High()
	{
		// Act & Assert
		((Func<string>)(() => LifxColor.BuildRGB(256, 0, 0)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildRGB_Should_Throw_On_Red_Too_Low()
	{
		// Act & Assert
		((Func<string>)(() => LifxColor.BuildRGB(-1, 0, 0)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildRGB_Should_Throw_On_Green_Too_High()
	{
		// Act & Assert
		((Func<string>)(() => LifxColor.BuildRGB(0, 256, 0)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildRGB_Should_Throw_On_Blue_Too_High()
	{
		// Act & Assert
		((Func<string>)(() => LifxColor.BuildRGB(0, 0, 256)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	#endregion

	#region LifxColor BuildHSBK Tests

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

	[Fact]
	public void BuildHSBK_Should_Throw_When_All_Null()
	{
		// Act & Assert
		((Func<string>)(() => LifxColor.BuildHSBK(null, null, null, null)))
			.Should()
			.ThrowExactly<ArgumentException>();
	}

	[Fact]
	public void BuildHSBK_Should_Validate_Hue_Range()
	{
		// Act & Assert - Too low
		((Func<string>)(() => LifxColor.BuildHSBK(-1, 0.5, 0.5, 3500)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();

		// Too high
		((Func<string>)(() => LifxColor.BuildHSBK(361, 0.5, 0.5, 3500)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildHSBK_Should_Validate_Saturation_Range()
	{
		// Act & Assert - Too low
		((Func<string>)(() => LifxColor.BuildHSBK(120, -0.1, 0.5, 3500)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();

		// Too high
		((Func<string>)(() => LifxColor.BuildHSBK(120, 1.1, 0.5, 3500)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildHSBK_Should_Validate_Brightness_Range()
	{
		// Act & Assert - Too low
		((Func<string>)(() => LifxColor.BuildHSBK(120, 0.5, -0.1, 3500)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();

		// Too high
		((Func<string>)(() => LifxColor.BuildHSBK(120, 0.5, 1.1, 3500)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildHSBK_Should_Validate_Kelvin_Range()
	{
		// Act & Assert - Too low
		((Func<string>)(() => LifxColor.BuildHSBK(120, 0.5, 0.5, 1499)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();

		// Too high
		((Func<string>)(() => LifxColor.BuildHSBK(120, 0.5, 0.5, 9001)))
			.Should()
			.ThrowExactly<InvalidConstraintException>();
	}

	[Fact]
	public void BuildHSBK_Should_Allow_Min_Kelvin()
	{
		// Act
		var color = LifxColor.BuildHSBK(null, null, 0.5, 1500);

		// Assert
		color.Should().Contain("kelvin:1500");
	}

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

	[Fact]
	public void NamedColors_Should_Have_Nine_Colors()
	{
		// Assert
		LifxColor.NamedColors.Should().HaveCount(9);
	}

	[Fact]
	public void DefaultWhite_Should_Be_White_String()
	{
		// Assert
		LifxColor.DefaultWhite.Should().Be("white");
	}

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
