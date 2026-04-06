using AwesomeAssertions;
using Lifx.Cli.Handlers;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for EffectsHandler request builders.
/// </summary>
[Collection("Unit Tests")]
public class EffectsHandlerTests
{
	/// <summary>
	/// Tests BuildBreatheRequest sets all properties correctly.
	/// </summary>
	[Fact]
	public void BuildBreatheRequest_SetsAllProperties()
	{
		var request = EffectsHandler.BuildBreatheRequest("red", 2.0, 5.0);

		request.Color.Should().Be("red");
		request.Period.Should().Be(2.0);
		request.Cycles.Should().Be(5.0);
		request.PowerOn.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildPulseRequest sets all properties correctly.
	/// </summary>
	[Fact]
	public void BuildPulseRequest_SetsAllProperties()
	{
		var request = EffectsHandler.BuildPulseRequest("blue", 1.5, 3.0);

		request.Color.Should().Be("blue");
		request.Period.Should().Be(1.5);
		request.Cycles.Should().Be(3.0);
		request.PowerOn.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildMorphRequest sets all properties correctly.
	/// </summary>
	[Fact]
	public void BuildMorphRequest_SetsAllProperties()
	{
		var request = EffectsHandler.BuildMorphRequest(3.0, 60.0);

		request.Period.Should().Be(3.0);
		request.Duration.Should().Be(60.0);
		request.PowerOn.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildFlameRequest sets all properties correctly.
	/// </summary>
	[Fact]
	public void BuildFlameRequest_SetsAllProperties()
	{
		var request = EffectsHandler.BuildFlameRequest(2.0, 30.0);

		request.Period.Should().Be(2.0);
		request.Duration.Should().Be(30.0);
		request.PowerOn.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildMoveRequest sets all properties correctly.
	/// </summary>
	[Fact]
	public void BuildMoveRequest_SetsAllProperties()
	{
		var request = EffectsHandler.BuildMoveRequest("forward", 1.0);

		request.Direction.Should().Be("forward");
		request.Period.Should().Be(1.0);
		request.PowerOn.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildCloudsRequest sets all properties correctly.
	/// </summary>
	[Fact]
	public void BuildCloudsRequest_SetsAllProperties()
	{
		var request = EffectsHandler.BuildCloudsRequest(120.0);

		request.Duration.Should().Be(120.0);
		request.PowerOn.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildSunriseRequest sets duration correctly.
	/// </summary>
	[Fact]
	public void BuildSunriseRequest_SetsDuration()
	{
		var request = EffectsHandler.BuildSunriseRequest(300.0);

		request.Duration.Should().Be(300.0);
	}

	/// <summary>
	/// Tests BuildSunsetRequest sets duration correctly.
	/// </summary>
	[Fact]
	public void BuildSunsetRequest_SetsDuration()
	{
		var request = EffectsHandler.BuildSunsetRequest(600.0);

		request.Duration.Should().Be(600.0);
	}

	/// <summary>
	/// Tests BuildOffRequest with power off true.
	/// </summary>
	[Fact]
	public void BuildOffRequest_SetsPowerOff()
	{
		var request = EffectsHandler.BuildOffRequest(true);

		request.PowerOff.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildOffRequest with power off false.
	/// </summary>
	[Fact]
	public void BuildOffRequest_WithPowerOffFalse()
	{
		var request = EffectsHandler.BuildOffRequest(false);

		request.PowerOff.Should().BeFalse();
	}
}
