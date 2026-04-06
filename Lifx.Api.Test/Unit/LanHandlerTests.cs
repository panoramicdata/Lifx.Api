using AwesomeAssertions;
using Lifx.Cli.Handlers;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for LanHandler validation and normalization.
/// </summary>
[Collection("Unit Tests")]
public class LanHandlerTests
{
	/// <summary>
	/// Tests NormalizeMacAddress with various input formats.
	/// </summary>
	[Theory]
	[InlineData("d0:73:d5:12:34:56", "D0:73:D5:12:34:56")]
	[InlineData("D0:73:D5:12:34:56", "D0:73:D5:12:34:56")]
	[InlineData("d0-73-d5-12-34-56", "D0:73:D5:12:34:56")]
	[InlineData("d0.73.d5.12.34.56", "D0:73:D5:12:34:56")]
	public void NormalizeMacAddress_VariousFormats(string input, string expected)
	{
		var result = LanHandler.NormalizeMacAddress(input);

		result.Should().Be(expected);
	}

	/// <summary>
	/// Tests ValidateKelvin with valid values does not throw.
	/// </summary>
	[Theory]
	[InlineData(2500)]
	[InlineData(5000)]
	[InlineData(9000)]
	public void ValidateKelvin_ValidValues_DoesNotThrow(int kelvin)
	{
		var act = () => LanHandler.ValidateKelvin(kelvin);

		act.Should().NotThrow();
	}

	/// <summary>
	/// Tests ValidateKelvin with out of range values throws.
	/// </summary>
	[Theory]
	[InlineData(2499)]
	[InlineData(9001)]
	[InlineData(0)]
	[InlineData(-1)]
	public void ValidateKelvin_OutOfRange_Throws(int kelvin)
	{
		var act = () => LanHandler.ValidateKelvin(kelvin);

		act.Should().Throw<ArgumentOutOfRangeException>()
			.WithParameterName(nameof(kelvin));
	}

	/// <summary>
	/// Tests ValidateLightName with valid names does not throw.
	/// </summary>
	[Theory]
	[InlineData("Bedroom")]
	[InlineData("A")]
	[InlineData("12345678901234567890123456789012")]
	public void ValidateLightName_ValidNames_DoesNotThrow(string name)
	{
		var act = () => LanHandler.ValidateLightName(name);

		act.Should().NotThrow();
	}

	/// <summary>
	/// Tests ValidateLightName with empty string throws.
	/// </summary>
	[Fact]
	public void ValidateLightName_Empty_Throws()
	{
		var act = () => LanHandler.ValidateLightName("");

		act.Should().Throw<ArgumentException>()
			.WithParameterName("name");
	}

	/// <summary>
	/// Tests ValidateLightName with whitespace throws.
	/// </summary>
	[Fact]
	public void ValidateLightName_Whitespace_Throws()
	{
		var act = () => LanHandler.ValidateLightName("   ");

		act.Should().Throw<ArgumentException>()
			.WithParameterName("name");
	}

	/// <summary>
	/// Tests ValidateLightName with name too long throws.
	/// </summary>
	[Fact]
	public void ValidateLightName_TooLong_Throws()
	{
		var act = () => LanHandler.ValidateLightName(new string('A', 33));

		act.Should().Throw<ArgumentException>()
			.WithParameterName("name");
	}
}
