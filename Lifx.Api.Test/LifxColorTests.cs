using AwesomeAssertions;
using System.Data;

namespace Lifx.Api.Test;

[Collection("Unit Tests")]
public class LifxColorTests
{
	[Fact]
	public void BuildHSBK_With_All_Parameters_Should_Format_Correctly()
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
	public void BuildRGB_Should_Format_Correctly()
	{
		// Act
		var color = LifxColor.BuildRGB(255, 0, 0);

		// Assert
		color.Should().Be("rgb:255,0,0");
	}

	[Fact]
	public void BuildRGB_With_Invalid_Red_Should_Throw() =>
		// Act & Assert
		Assert.Throws<InvalidConstraintException>(() => LifxColor.BuildRGB(256, 0, 0));

	[Fact]
	public void BuildHSBK_With_Invalid_Hue_Should_Throw() =>
		// Act & Assert
		Assert.Throws<InvalidConstraintException>(() => LifxColor.BuildHSBK(361, 0.5, 0.5, 3500));

	[Fact]
	public void BuildHSBK_With_Invalid_Saturation_Should_Throw() =>
		// Act & Assert
		Assert.Throws<InvalidConstraintException>(() => LifxColor.BuildHSBK(120, 1.5, 0.5, 3500));

	[Fact]
	public void Named_Colors_Should_Contain_Expected_Values()
	{
		// Assert
		LifxColor.NamedColors.Should().Contain("green");
		LifxColor.NamedColors.Should().Contain("blue");
		LifxColor.NamedColors.Should().Contain("red");
	}
}
