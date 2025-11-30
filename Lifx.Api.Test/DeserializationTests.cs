using AwesomeAssertions;
using Lifx.Api.Models.Cloud.Responses;
using System.Text.Json;

namespace Lifx.Api.Test;

[Collection("Model Tests")]
public class DeserializationTests
{
	[Fact]
	public void Light_Should_Deserialize_From_Json()
	{
		// Arrange - Sample JSON from LIFX API
		var json = @"{
			""id"": ""d073d5000001"",
			""uuid"": ""8fa5f072-af97-44ed-ae54-e70fd7bd9d20"",
			""label"": ""Test Light"",
			""connected"": true,
			""power"": ""on"",
			""color"": {
				""hue"": 120.0,
				""saturation"": 1.0,
				""brightness"": 0.5,
				""kelvin"": 3500
			},
			""brightness"": 0.5,
			""group"": {
				""id"": ""group123"",
				""name"": ""Living Room""
			},
			""location"": {
				""id"": ""location456"",
				""name"": ""Home""
			},
			""product"": {
				""name"": ""LIFX Color 1000"",
				""identifier"": ""lifx_color_a19"",
				""company"": ""LIFX"",
				""capabilities"": {
					""has_color"": true,
					""has_variable_color_temp"": true
				}
			},
			""last_seen"": ""2024-01-15T10:30:00Z"",
			""seconds_since_seen"": 5.0,
			""product_name"": ""LIFX Color 1000"",
			""capabilities"": {
				""has_color"": true,
				""has_variable_color_temp"": true
			}
		}";

		// Act
		var light = JsonSerializer.Deserialize<Light>(json, LifxClient.JsonSerializerOptions);

		// Assert
		light.Should().NotBeNull();
		light.Id.Should().Be("d073d5000001");
		light.Label.Should().Be("Test Light");
		light.IsConnected.Should().BeTrue();
		light.PowerState.Should().Be(PowerState.On);
		light.IsOn.Should().BeTrue();
		light.Brightness.Should().Be(0.5f);
		light.GroupId.Should().Be("group123");
		light.GroupName.Should().Be("Living Room");
		light.LocationId.Should().Be("location456");
		light.LocationName.Should().Be("Home");
		light.Color.Should().NotBeNull();
		light.Color.Hue.Should().Be(120.0f);
		light.HasCapability("has_color").Should().BeTrue();
		light.HasCapability("has_variable_color_temp").Should().BeTrue();
	}

	[Fact]
	public void Hsbk_Should_Deserialize_From_Json()
	{
		// Arrange
		var json = @"{
			""hue"": 240.0,
			""saturation"": 0.8,
			""brightness"": 0.6,
			""kelvin"": 4000
		}";

		// Act
		var hsbk = JsonSerializer.Deserialize<Hsbk>(json, LifxClient.JsonSerializerOptions);

		// Assert
		hsbk.Should().NotBeNull();
		hsbk.Hue.Should().Be(240.0f);
		hsbk.Saturation.Should().Be(0.8f);
		hsbk.Brightness.Should().Be(0.6f);
		hsbk.Kelvin.Should().Be(4000);
	}

	[Fact]
	public void Light_With_Null_Color_Should_Deserialize()
	{
		// Arrange - Light with null color
		var json = @"{
			""id"": ""test123"",
			""uuid"": ""uuid123"",
			""label"": ""Test"",
			""connected"": false,
			""power"": ""off"",
			""color"": null,
			""brightness"": 0.0,
			""group"": {""id"": ""g1"", ""name"": ""Group""},
			""location"": {""id"": ""l1"", ""name"": ""Location""},
			""last_seen"": null,
			""seconds_since_seen"": 0.0,
			""product_name"": ""Test Product"",
			""capabilities"": {}
		}";

		// Act
		var light = JsonSerializer.Deserialize<Light>(json, LifxClient.JsonSerializerOptions);

		// Assert
		light.Should().NotBeNull();
		light.Id.Should().Be("test123");
		light.Color.Should().BeNull();
		light.LastSeen.Should().BeNull();
	}

	[Fact]
	public void PowerState_Should_Serialize_And_Deserialize()
	{
		// Test both directions
		var onJson = JsonSerializer.Serialize(PowerState.On, LifxClient.JsonSerializerOptions);
		var offJson = JsonSerializer.Serialize(PowerState.Off, LifxClient.JsonSerializerOptions);

		onJson.Should().Be("\"on\"");
		offJson.Should().Be("\"off\"");

		var onParsed = JsonSerializer.Deserialize<PowerState>("\"on\"", LifxClient.JsonSerializerOptions);
		var offParsed = JsonSerializer.Deserialize<PowerState>("\"off\"", LifxClient.JsonSerializerOptions);

		onParsed.Should().Be(PowerState.On);
		offParsed.Should().Be(PowerState.Off);
	}
}
