using AwesomeAssertions;
using Lifx.Api.Models.Cloud.Responses;
using System.Text.Json;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Phase 4: Model Validation & Serialization Tests
/// Tests all model serialization/deserialization and validation
/// </summary>
[Collection("Unit Tests")]
public class ModelSerializationTests
{
	#region Hsbk Tests

	[Fact]
	public void Hsbk_Should_Serialize_All_Components()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = 240.0f,
			Saturation = 0.8f,
			Brightness = 0.6f,
			Kelvin = 3500
		};

		// Act
		var json = JsonSerializer.Serialize(hsbk, LifxClient.JsonSerializerOptions);
		var deserialized = JsonSerializer.Deserialize<Hsbk>(json, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Contain("\"hue\"");
		json.Should().Contain("\"saturation\"");
		json.Should().Contain("\"brightness\"");
		json.Should().Contain("\"kelvin\"");
		deserialized.Should().NotBeNull();
		deserialized!.Hue.Should().Be(240.0f);
		deserialized.Saturation.Should().Be(0.8f);
		deserialized.Brightness.Should().Be(0.6f);
		deserialized.Kelvin.Should().Be(3500);
	}

	[Fact]
	public void Hsbk_Should_ToString_With_All_Components()
	{
		// Arrange - Use very low saturation so kelvin is included
		var hsbk = new Hsbk
		{
			Hue = 120.0f,
			Saturation = 0.0f, // Low saturation so kelvin will be included
			Brightness = 0.75f,
			Kelvin = 4000
		};

		// Act
		var result = hsbk.ToString();

		// Assert
		result.Should().Contain("hue:120");
		result.Should().Contain("saturation:0");
		result.Should().Contain("brightness:0.75");
		result.Should().Contain("kelvin:4000");
	}

	[Fact]
	public void Hsbk_ToString_Should_Handle_Null_Hue()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = null,
			Saturation = 0.5f,
			Brightness = 0.5f,
			Kelvin = 3500
		};

		// Act
		var result = hsbk.ToString();

		// Assert
		result.Should().NotContain("hue:");
		result.Should().Contain("saturation:0.5");
	}

	[Fact]
	public void Hsbk_Should_Deserialize_With_Null_Components()
	{
		// Arrange
		var json = """
{
	"hue": null,
	"saturation": 0.8,
	"brightness": 0.6,
	"kelvin": 3500
}
""";

		// Act
		var hsbk = JsonSerializer.Deserialize<Hsbk>(json, LifxClient.JsonSerializerOptions);

		// Assert
		hsbk.Should().NotBeNull();
		hsbk!.Hue.Should().BeNull();
		hsbk.Saturation.Should().Be(0.8f);
		hsbk.Brightness.Should().Be(0.6f);
		hsbk.Kelvin.Should().Be(3500);
	}

	#endregion

	#region Selector Tests

	[Fact]
	public void Selector_Should_Parse_All_Selector()
	{
		// Act
		var selector = (Selector)"all";

		// Assert
		selector.ToString().Should().Be("all");
		selector.Should().Be(Selector.All);
	}

	[Theory]
	[InlineData("id:d073d5000001", "id:d073d5000001")]
	[InlineData("group_id:abc123", "group_id:abc123")]
	[InlineData("group:Living Room", "group:Living Room")]
	[InlineData("location_id:xyz789", "location_id:xyz789")]
	[InlineData("location:Home", "location:Home")]
	[InlineData("label:Kitchen", "label:Kitchen")]
	public void Selector_Should_Parse_All_Types(string input, string expected)
	{
		// Act
		var selector = (Selector)input;

		// Assert
		selector.ToString().Should().Be(expected);
	}

	[Fact]
	public void Selector_LightId_Should_Format_Correctly()
	{
		// Arrange & Act
		var selector = new Selector.LightId("test123");

		// Assert
		selector.ToString().Should().Be("id:test123");
	}

	[Fact]
	public void Selector_GroupId_Should_Format_Correctly()
	{
		// Arrange & Act
		var selector = new Selector.GroupId("group456");

		// Assert
		selector.ToString().Should().Be("group_id:group456");
	}

	[Fact]
	public void Selector_LocationId_Should_Format_Correctly()
	{
		// Arrange & Act
		var selector = new Selector.LocationId("loc789");

		// Assert
		selector.ToString().Should().Be("location_id:loc789");
	}

	#endregion

	#region CollectionSpec Tests

	[Fact]
	public void CollectionSpec_Should_Serialize_Correctly()
	{
		// Arrange
		var spec = new CollectionSpec
		{
			Id = "test-id",
			Name = "Test Name"
		};

		// Act
		var json = JsonSerializer.Serialize(spec, LifxClient.JsonSerializerOptions);
		var deserialized = JsonSerializer.Deserialize<CollectionSpec>(json, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Contain("\"id\"");
		json.Should().Contain("\"name\"");
		deserialized.Should().NotBeNull();
		deserialized.Id.Should().Be("test-id");
		deserialized.Name.Should().Be("Test Name");
	}

	[Fact]
	public void CollectionSpec_Equals_Should_Work_Correctly()
	{
		// Arrange
		var spec1 = new CollectionSpec { Id = "id1", Name = "name1" };
		var spec2 = new CollectionSpec { Id = "id1", Name = "name1" };
		var spec3 = new CollectionSpec { Id = "id2", Name = "name2" };

		// Assert
		spec1.Equals(spec2).Should().BeTrue();
		spec1.Equals(spec3).Should().BeFalse();
		spec1.GetHashCode().Should().Be(spec2.GetHashCode());
	}

	#endregion

	#region ApiResponse Models Tests

	[Fact]
	public void ErrorResponse_Should_Deserialize_With_Single_Error()
	{
		// Arrange
		var json = """
{
	"error": "Invalid selector"
}
""";

		// Act
		var response = JsonSerializer.Deserialize<ErrorResponse>(json, LifxClient.JsonSerializerOptions);

		// Assert
		response.Should().NotBeNull();
		response.Error.Should().Be("Invalid selector");
	}

	[Fact]
	public void ErrorResponse_Should_Deserialize_With_Error_Array()
	{
		// Arrange
		var json = """
{
	"errors": [
		{
			"field": "color",
			"message": ["Color is invalid"]
		}
	]
}
""";

		// Act
		var response = JsonSerializer.Deserialize<ErrorResponse>(json, LifxClient.JsonSerializerOptions);

		// Assert
		response.Should().NotBeNull();
		response.Errors.Should().NotBeNull();
		response.Errors.Should().ContainSingle();
		response.Errors![0].Field.Should().Be("color");
	}

	[Fact]
	public void Result_Should_Identify_Successful_Status()
	{
		// Arrange
		var result = new Result
		{
			Id = "d073d5000001",
			Label = "Test Light",
			Status = "ok"
		};

		// Assert
		result.IsSuccessful.Should().BeTrue();
		result.IsTimedOut.Should().BeFalse();
	}

	[Fact]
	public void Result_Should_Identify_TimedOut_Status()
	{
		// Arrange
		var result = new Result
		{
			Id = "d073d5000001",
			Label = "Test Light",
			Status = "timed_out"
		};

		// Assert
		result.IsSuccessful.Should().BeFalse();
		result.IsTimedOut.Should().BeTrue();
	}

	#endregion

	#region Scene Models Tests

	[Fact]
	public void Scene_Should_Deserialize_Complete_Model()
	{
		// Arrange
		var json = """
{
	"uuid": "scene-123",
	"name": "Evening Mode",
	"account": {
		"uuid": "account-456"
	},
	"states": [
		{
			"selector": "all",
			"brightness": 0.5,
			"color": {
				"hue": 120.0,
				"saturation": 1.0,
				"brightness": 0.5,
				"kelvin": 3500
			}
		}
	],
	"created_at": 1234567890,
	"updated_at": 1234567900
}
""";

		// Act
		var scene = JsonSerializer.Deserialize<Scene>(json, LifxClient.JsonSerializerOptions);

		// Assert
		scene.Should().NotBeNull();
		scene!.Uuid.Should().Be("scene-123");
		scene.Name.Should().Be("Evening Mode");
		scene.Account.UUID.Should().Be("account-456");
		scene.States.Should().ContainSingle();
		scene.CreatedAt.Should().Be(1234567890);
		scene.UpdatedAt.Should().Be(1234567900);
	}

	[Fact]
	public void Scene_State_Should_Deserialize_Correctly()
	{
		// Arrange
		var json = """
{
	"selector": "group:Living Room",
	"brightness": 0.75,
	"color": {
		"hue": 240.0,
		"saturation": 0.8,
		"brightness": 0.6,
		"kelvin": 4000
	}
}
""";

		// Act
		var state = JsonSerializer.Deserialize<State>(json, LifxClient.JsonSerializerOptions);

		// Assert
		state.Should().NotBeNull();
		state!.Selector.Should().Be("group:Living Room");
		state.Brightness.Should().Be(0.75f);
		state.Color.Should().NotBeNull();
		state.Color!.Hue.Should().Be(240.0f);
	}

	#endregion

	#region ColorResult Tests

	[Fact]
	public void ColorResult_Should_Deserialize_All_Fields()
	{
		// Arrange
		var json = """
{
	"hue": 120,
	"saturation": 1.0,
	"brightness": 0.5,
	"kelvin": 3500
}
""";

		// Act
		var result = JsonSerializer.Deserialize<ColorResult>(json, LifxClient.JsonSerializerOptions);

		// Assert
		result.Should().NotBeNull();
		result!.Hue.Should().Be(120);
		result.Saturation.Should().Be(1.0f);
		result.Brightness.Should().Be(0.5f);
		result.Kelvin.Should().Be(3500f);
	}

	[Fact]
	public void ColorResult_Should_Handle_Null_Values()
	{
		// Arrange
		var json = """
{
	"hue": null,
	"saturation": null,
	"brightness": null,
	"kelvin": null
}
""";

		// Act
		var result = JsonSerializer.Deserialize<ColorResult>(json, LifxClient.JsonSerializerOptions);

		// Assert
		result.Should().NotBeNull();
		result!.Hue.Should().BeNull();
		result.Saturation.Should().BeNull();
		result.Brightness.Should().BeNull();
		result.Kelvin.Should().BeNull();
	}

	#endregion

	#region Request Models Tests

	[Fact]
	public void SetStateRequest_Should_Serialize_All_Fields()
	{
		// Arrange
		var request = new SetStateRequest
		{
			Power = PowerState.On,
			Color = "blue",
			Brightness = 0.75,
			Duration = 2.5,
			Infrared = 0.5
		};

		// Act
		var json = JsonSerializer.Serialize(request, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Contain("\"power\":\"on\"");
		json.Should().Contain("\"color\":\"blue\"");
		json.Should().Contain("\"brightness\":0.75");
		json.Should().Contain("\"duration\":2.5");
		json.Should().Contain("\"infrared\":0.5");
	}

	[Fact]
	public void SetStateRequest_Should_Omit_Null_Values()
	{
		// Arrange
		var request = new SetStateRequest
		{
			Power = PowerState.On
			// All other fields null
		};

		// Act
		var json = JsonSerializer.Serialize(request, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Contain("\"power\"");
		json.Should().NotContain("\"color\"");
		json.Should().NotContain("\"brightness\"");
	}

	[Fact]
	public void TogglePowerRequest_Should_Serialize_Duration()
	{
		// Arrange
		var request = new TogglePowerRequest
		{
			Duration = 1.5
		};

		// Act
		var json = JsonSerializer.Serialize(request, LifxClient.JsonSerializerOptions);

		// Assert
		json.Should().Contain("\"duration\":1.5");
	}

	#endregion

	#region Enum Serialization Tests

	[Fact]
	public void PowerState_Should_Serialize_Consistently()
	{
		// Act
		var onJson = JsonSerializer.Serialize(PowerState.On, LifxClient.JsonSerializerOptions);
		var offJson = JsonSerializer.Serialize(PowerState.Off, LifxClient.JsonSerializerOptions);

		// Assert
		onJson.Should().Be("\"on\"");
		offJson.Should().Be("\"off\"");
	}

	[Fact]
	public void PowerState_Should_Deserialize_Case_Insensitive()
	{
		// Act
		var on1 = JsonSerializer.Deserialize<PowerState>("\"on\"", LifxClient.JsonSerializerOptions);
		var on2 = JsonSerializer.Deserialize<PowerState>("\"ON\"", LifxClient.JsonSerializerOptions);
		var off1 = JsonSerializer.Deserialize<PowerState>("\"off\"", LifxClient.JsonSerializerOptions);
		var off2 = JsonSerializer.Deserialize<PowerState>("\"OFF\"", LifxClient.JsonSerializerOptions);

		// Assert
		on1.Should().Be(PowerState.On);
		on2.Should().Be(PowerState.On);
		off1.Should().Be(PowerState.Off);
		off2.Should().Be(PowerState.Off);
	}

	#endregion

	#region Null Handling Tests

	[Fact]
	public void Light_Should_Handle_Null_Color()
	{
		// Arrange
		var json = """
{
	"id": "test123",
	"uuid": "uuid123",
	"label": "Test Light",
	"connected": true,
	"power": "on",
	"color": null,
	"brightness": 0.5,
	"group": {"id": "g1", "name": "Group 1"},
	"location": {"id": "l1", "name": "Location 1"},
	"last_seen": "2024-01-15T10:30:00Z",
	"seconds_since_seen": 5.0,
	"product_name": "LIFX Color",
	"capabilities": {"has_color": true}
}
""";

		// Act
		var light = JsonSerializer.Deserialize<Light>(json, LifxClient.JsonSerializerOptions);

		// Assert
		light.Should().NotBeNull();
		light.Color.Should().BeNull();
	}

	[Fact]
	public void Light_Should_Handle_Missing_LastSeen()
	{
		// Arrange
		var json = """
{
	"id": "test123",
	"uuid": "uuid123",
	"label": "Test Light",
	"connected": false,
	"power": "off",
	"brightness": 0.0,
	"group": {"id": "g1", "name": "Group 1"},
	"location": {"id": "l1", "name": "Location 1"},
	"seconds_since_seen": 0.0,
	"product_name": "LIFX Color",
	"capabilities": {}
}
""";

		// Act
		var light = JsonSerializer.Deserialize<Light>(json, LifxClient.JsonSerializerOptions);

		// Assert
		light.Should().NotBeNull();
		light.LastSeen.Should().BeNull();
	}

	[Fact]
	public void Light_Should_Handle_EmptyString_LastSeen()
	{
		// Arrange - LIFX API returns empty string for disconnected lights
		// See: https://github.com/panoramicdata/Lifx.Api/issues/3
		var json = """
{
	"id": "test123",
	"uuid": "uuid123",
	"label": "Test Light",
	"connected": false,
	"power": "off",
	"brightness": 0.0,
	"group": {"id": "g1", "name": "Group 1"},
	"location": {"id": "l1", "name": "Location 1"},
	"last_seen": "",
	"seconds_since_seen": 0.0,
	"product_name": "LIFX Color",
	"capabilities": {}
}
""";

		// Act
		var light = JsonSerializer.Deserialize<Light>(json, LifxClient.JsonSerializerOptions);

		// Assert
		light.Should().NotBeNull();
		light.LastSeen.Should().BeNull();
	}

	#endregion

	#region Default Values Tests

	[Fact]
	public void CollectionSpec_Should_Have_Default_Empty_Strings()
	{
		// Arrange & Act
		var spec = new CollectionSpec();

		// Assert
		spec.Id.Should().Be(string.Empty);
		spec.Name.Should().Be(string.Empty);
	}

	[Fact]
	public void Hsbk_Default_Values_Should_Be_Null()
	{
		// Arrange & Act
		var hsbk = new Hsbk();

		// Assert
		hsbk.Hue.Should().BeNull();
		hsbk.Saturation.Should().BeNull();
		hsbk.Brightness.Should().BeNull();
		hsbk.Kelvin.Should().BeNull();
	}

	#endregion
}
