using AwesomeAssertions;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Phase 5: Error Handling & Edge Cases Tests
/// Tests validation, error scenarios, and edge cases
/// </summary>
[Collection("Unit Tests")]
public class ValidationTests
{
	#region Request Validation Tests

	[Fact]
	public void SetStateRequest_Should_Accept_Valid_Duration()
	{
		// Arrange & Act
		var request = new SetStateRequest
		{
			Duration = 1.5
		};

		// Assert
		request.Duration.Should().Be(1.5);
	}

	[Fact]
	public void SetStateRequest_Should_Accept_Zero_Duration()
	{
		// Arrange & Act
		var request = new SetStateRequest
		{
			Duration = 0.0
		};

		// Assert
		request.Duration.Should().Be(0.0);
	}

	[Fact]
	public void SetStateRequest_Should_Have_Default_Duration()
	{
		// Arrange & Act
		var request = new SetStateRequest();

		// Assert
		request.Duration.Should().Be(1.0);
	}

	[Fact]
	public void TogglePowerRequest_Should_Accept_Valid_Duration()
	{
		// Arrange & Act
		var request = new TogglePowerRequest
		{
			Duration = 2.5
		};

		// Assert
		request.Duration.Should().Be(2.5);
	}

	#endregion

	#region Color Value Validation Tests

	[Fact]
	public void LifxColor_BuildRGB_Should_Reject_Red_Below_Zero()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildRGB(-1, 0, 0));
	}

	[Fact]
	public void LifxColor_BuildRGB_Should_Reject_Red_Above_255()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildRGB(256, 0, 0));
	}

	[Fact]
	public void LifxColor_BuildRGB_Should_Reject_Green_Below_Zero()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildRGB(0, -1, 0));
	}

	[Fact]
	public void LifxColor_BuildRGB_Should_Reject_Green_Above_255()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildRGB(0, 256, 0));
	}

	[Fact]
	public void LifxColor_BuildRGB_Should_Reject_Blue_Below_Zero()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildRGB(0, 0, -1));
	}

	[Fact]
	public void LifxColor_BuildRGB_Should_Reject_Blue_Above_255()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildRGB(0, 0, 256));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Hue_Below_Zero()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(-1, 1.0, 1.0, 3500));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Hue_Above_360()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(361, 1.0, 1.0, 3500));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Saturation_Below_Zero()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(180, -0.1, 1.0, 3500));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Saturation_Above_One()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(180, 1.1, 1.0, 3500));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Brightness_Below_Zero()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(180, 1.0, -0.1, 3500));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Brightness_Above_One()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(180, 1.0, 1.1, 3500));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Kelvin_Below_Min()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(180, 0.0, 1.0, LifxColor.TemperatureMin - 1));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Reject_Kelvin_Above_Max()
	{
		// Act & Assert
		Assert.Throws<System.Data.InvalidConstraintException>(() =>
			LifxColor.BuildHSBK(180, 0.0, 1.0, LifxColor.TemperatureMax + 1));
	}

	[Fact]
	public void LifxColor_BuildHSBK_Should_Throw_When_All_Null()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() =>
			LifxColor.BuildHSBK(null, null, null, null));
	}

	#endregion

	#region Selector Validation Tests

	[Fact]
	public void Selector_LightId_Should_Accept_Valid_Id()
	{
		// Act
		var selector = new Selector.LightId("d073d5000001");

		// Assert
		selector.ToString().Should().Be("id:d073d5000001");
	}

	[Fact]
	public void Selector_GroupLabel_Should_Accept_Spaces()
	{
		// Act
		var selector = new Selector.GroupLabel("Living Room");

		// Assert
		selector.ToString().Should().Be("group:Living Room");
	}

	[Fact]
	public void Selector_LocationLabel_Should_Accept_Special_Characters()
	{
		// Act
		var selector = new Selector.LocationLabel("Mom's House");

		// Assert
		selector.ToString().Should().Be("location:Mom's House");
	}

	[Fact]
	public void Selector_Cast_Should_Handle_Plain_Label()
	{
		// Act
		var selector = (Selector)"Kitchen Light";

		// Assert
		selector.ToString().Should().Be("label:Kitchen Light");
	}

	[Fact]
	public void Selector_Cast_Should_Handle_Label_With_Colon()
	{
		// Act
		var selector = (Selector)"Test: Light";

		// Assert - Should treat as label since format is invalid
		selector.ToString().Should().Be("label:Test: Light");
	}

	#endregion

	#region Edge Cases Tests

	[Fact]
	public void Hsbk_ToString_Should_Clamp_Hue_To_Range()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = 400.0f, // Above max
			Saturation = 0.5f,
			Brightness = 0.5f,
			Kelvin = 3500
		};

		// Act
		var result = hsbk.ToString();

		// Assert - Should clamp to 360
		result.Should().Contain("hue:360");
	}

	[Fact]
	public void Hsbk_ToString_Should_Clamp_Saturation_To_Range()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = 180.0f,
			Saturation = 1.5f, // Above max
			Brightness = 0.5f
		};

		// Act
		var result = hsbk.ToString();

		// Assert - Should clamp to 1
		result.Should().Contain("saturation:1");
	}

	[Fact]
	public void Hsbk_ToString_Should_Clamp_Brightness_To_Range()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = 180.0f,
			Saturation = 0.5f,
			Brightness = -0.1f // Below min
		};

		// Act
		var result = hsbk.ToString();

		// Assert - Should clamp to 0
		result.Should().Contain("brightness:0");
	}

	[Fact]
	public void Hsbk_ToString_Should_Clamp_Kelvin_To_Range()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = 180.0f,
			Saturation = 0.0f, // Low so kelvin is included
			Brightness = 0.5f,
			Kelvin = 10000 // Above max
		};

		// Act
		var result = hsbk.ToString();

		// Assert - Should clamp to max (9000)
		result.Should().Contain($"kelvin:{LifxColor.TemperatureMax}");
	}

	[Fact]
	public void Hsbk_ToString_Should_Omit_Kelvin_When_Saturation_High()
	{
		// Arrange
		var hsbk = new Hsbk
		{
			Hue = 180.0f,
			Saturation = 0.5f, // High enough to omit kelvin
			Brightness = 0.5f,
			Kelvin = 3500
		};

		// Act
		var result = hsbk.ToString();

		// Assert
		result.Should().NotContain("kelvin:");
	}

	[Fact]
	public void Light_Implicit_Operator_Should_Create_LightId_Selector()
	{
		// Arrange - Deserialize to create a Light with an ID
		var json = """
{
	"id": "test123",
	"uuid": "uuid123",
	"label": "Test",
	"connected": true,
	"power": "on",
	"brightness": 0.5,
	"group": {"id": "g1", "name": "Group"},
	"location": {"id": "l1", "name": "Location"},
	"seconds_since_seen": 0.0,
	"product_name": "Test"
}
""";
		var light = System.Text.Json.JsonSerializer.Deserialize<Light>(json, LifxClient.JsonSerializerOptions);

		// Act
		Selector selector = light!;

		// Assert
		selector.ToString().Should().Be("id:test123");
	}

	[Fact]
	public void LightCollection_Implicit_Operator_Should_Use_ToSelector()
	{
		// Arrange
		var lights = new List<Light>();
		var group = new Group("group123", "Test Group", lights);

		// Act
		Selector selector = group;

		// Assert
		selector.ToString().Should().Be("group_id:group123");
	}

	[Fact]
	public void CollectionSpec_Should_Handle_Empty_Strings()
	{
		// Arrange & Act
		var spec = new CollectionSpec
		{
			Id = "",
			Name = ""
		};

		// Assert
		spec.Id.Should().Be(string.Empty);
		spec.Name.Should().Be(string.Empty);
	}

	[Fact]
	public void Result_Status_Should_Be_Case_Sensitive()
	{
		// Arrange
		var okResult = new Result { Id = "1", Label = "Test", Status = "ok" };
		var OkResult = new Result { Id = "1", Label = "Test", Status = "Ok" };

		// Assert
		okResult.IsSuccessful.Should().BeTrue();
		OkResult.IsSuccessful.Should().BeFalse(); // Case sensitive
	}

	#endregion

	#region Null/Empty Handling Tests

	[Fact]
	public void SetStateRequest_Should_Handle_Null_Color()
	{
		// Arrange & Act
		var request = new SetStateRequest
		{
			Power = PowerState.On,
			Color = null
		};

		// Assert
		request.Color.Should().BeNull();
	}

	[Fact]
	public void SetStateRequest_Should_Handle_Empty_Color()
	{
		// Arrange & Act
		var request = new SetStateRequest
		{
			Power = PowerState.On,
			Color = ""
		};

		// Assert
		request.Color.Should().Be(string.Empty);
	}

	[Fact]
	public void Hsbk_Should_Handle_All_Null_Values()
	{
		// Arrange & Act
		var hsbk = new Hsbk
		{
			Hue = null,
			Saturation = null,
			Brightness = null,
			Kelvin = null
		};

		// Assert
		hsbk.ToString().Should().Be(string.Empty);
	}

	#endregion
}
