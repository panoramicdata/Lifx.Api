using AwesomeAssertions;
using System.Text.Json;

namespace Lifx.Api.Test;

[Collection("Model Tests")]
public class PowerStateTests
{
	[Fact]
	public void PowerState_On_Should_Serialize_As_Lowercase()
	{
		// Arrange
		var powerState = PowerState.On;

		// Act
		var json = JsonSerializer.Serialize(powerState, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Be("\"on\"");
	}

	[Fact]
	public void PowerState_Off_Should_Serialize_As_Lowercase()
	{
		// Arrange
		var powerState = PowerState.Off;

		// Act
		var json = JsonSerializer.Serialize(powerState, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Be("\"off\"");
	}

	[Fact]
	public void PowerState_Should_Deserialize_On()
	{
		// Arrange
		var json = "\"on\"";

		// Act
		var result = JsonSerializer.Deserialize<PowerState>(json, LifxClient.JsonSerializerOptions);

		// Assert
		result.Should().Be(PowerState.On);
	}

	[Fact]
	public void PowerState_Should_Deserialize_Off()
	{
		// Arrange
		var json = "\"off\"";

		// Act
		var result = JsonSerializer.Deserialize<PowerState>(json, LifxClient.JsonSerializerOptions);

		// Assert
		result.Should().Be(PowerState.Off);
	}
}
