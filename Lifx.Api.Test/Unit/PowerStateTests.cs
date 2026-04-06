using AwesomeAssertions;
using System.Text.Json;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Represents the PowerStateTests type.
/// </summary>
[Collection("Model Tests")]
public class PowerStateTests
{
	/// <summary>
	/// Performs PowerState_On_Should_Serialize_As_Lowercase operation.
	/// </summary>
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

	/// <summary>
	/// Performs PowerState_Off_Should_Serialize_As_Lowercase operation.
	/// </summary>
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

	/// <summary>
	/// Performs PowerState_Should_Deserialize_On operation.
	/// </summary>
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

	/// <summary>
	/// Performs PowerState_Should_Deserialize_Off operation.
	/// </summary>
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
