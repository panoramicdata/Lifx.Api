using AwesomeAssertions;
using System.Data;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Represents the LifxColorTests type.
/// </summary>
[Collection("Unit Tests")]
public class LifxColorTests
{
	private static void AssertInvalid(Func<string> build)
		=> Assert.Throws<InvalidConstraintException>(build);

	/// <summary>
	/// Performs BuildHSBK_With_All_Parameters_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_With_All_Parameters_Should_Format_Correctly()
		=> LifxColor.BuildHSBK(120, 0.5, 0.8, 3500)
			.Should()
			.ContainAll("hue:120", "saturation:0.5", "brightness:0.8", "kelvin:3500");

	/// <summary>
	/// Performs BuildRGB_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void BuildRGB_Should_Format_Correctly()
		=> LifxColor.BuildRGB(255, 0, 0).Should().Be("rgb:255,0,0");

	/// <summary>
	/// Performs BuildRGB_With_Invalid_Red_Should_Throw operation.
	/// </summary>
	[Fact]
	public void BuildRGB_With_Invalid_Red_Should_Throw()
		=> AssertInvalid(() => LifxColor.BuildRGB(256, 0, 0));

	/// <summary>
	/// Performs BuildHSBK_With_Invalid_Hue_Should_Throw operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_With_Invalid_Hue_Should_Throw()
		=> AssertInvalid(() => LifxColor.BuildHSBK(361, 0.5, 0.5, 3500));

	/// <summary>
	/// Performs BuildHSBK_With_Invalid_Saturation_Should_Throw operation.
	/// </summary>
	[Fact]
	public void BuildHSBK_With_Invalid_Saturation_Should_Throw()
		=> AssertInvalid(() => LifxColor.BuildHSBK(120, 1.5, 0.5, 3500));

	/// <summary>
	/// Performs Named_Colors_Should_Contain_Expected_Values operation.
	/// </summary>
	[Fact]
	public void Named_Colors_Should_Contain_Expected_Values()
		=> LifxColor.NamedColors.Should().Contain(["green", "blue", "red"]);
}
