using AwesomeAssertions;
using Lifx.Api.Models.Cloud;
using Lifx.Cli.Handlers;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for LightsHandler request builders and validation.
/// </summary>
[Collection("Unit Tests")]
public class LightsHandlerTests
{
	/// <summary>
	/// Tests BuildOnRequest sets power on and duration.
	/// </summary>
	[Fact]
	public void BuildOnRequest_SetsPowerOnAndDuration()
	{
		var request = LightsHandler.BuildOnRequest(2.0);

		request.Power.Should().Be(PowerState.On);
		request.Duration.Should().Be(2.0);
	}

	/// <summary>
	/// Tests BuildOffRequest sets power off and duration.
	/// </summary>
	[Fact]
	public void BuildOffRequest_SetsPowerOffAndDuration()
	{
		var request = LightsHandler.BuildOffRequest(1.5);

		request.Power.Should().Be(PowerState.Off);
		request.Duration.Should().Be(1.5);
	}

	/// <summary>
	/// Tests BuildToggleRequest sets duration.
	/// </summary>
	[Fact]
	public void BuildToggleRequest_SetsDuration()
	{
		var request = LightsHandler.BuildToggleRequest(3.0);

		request.Duration.Should().Be(3.0);
	}

	/// <summary>
	/// Tests BuildColorRequest sets color and duration.
	/// </summary>
	[Fact]
	public void BuildColorRequest_SetsColorAndDuration()
	{
		var request = LightsHandler.BuildColorRequest("blue", 0.5);

		request.Color.Should().Be("blue");
		request.Duration.Should().Be(0.5);
	}

	/// <summary>
	/// Tests BuildBrightnessRequest with valid value sets properties.
	/// </summary>
	[Fact]
	public void BuildBrightnessRequest_ValidValue_SetsProperties()
	{
		var request = LightsHandler.BuildBrightnessRequest(0.75, 1.0);

		request.Brightness.Should().Be(0.75);
		request.Duration.Should().Be(1.0);
	}

	/// <summary>
	/// Tests BuildBrightnessRequest boundary values succeed.
	/// </summary>
	[Theory]
	[InlineData(0.0)]
	[InlineData(1.0)]
	public void BuildBrightnessRequest_BoundaryValues_Succeeds(double brightness)
	{
		var request = LightsHandler.BuildBrightnessRequest(brightness, 1.0);

		request.Brightness.Should().Be(brightness);
	}

	/// <summary>
	/// Tests BuildBrightnessRequest out of range throws.
	/// </summary>
	[Theory]
	[InlineData(-0.1)]
	[InlineData(1.1)]
	[InlineData(-1.0)]
	[InlineData(2.0)]
	public void BuildBrightnessRequest_OutOfRange_Throws(double brightness)
	{
		var act = () => LightsHandler.BuildBrightnessRequest(brightness, 1.0);

		act.Should().Throw<ArgumentOutOfRangeException>()
			.WithParameterName(nameof(brightness));
	}
}
