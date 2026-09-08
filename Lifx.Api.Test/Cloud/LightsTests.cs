using AwesomeAssertions;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

/// <summary>
/// Represents the LightsTests type.
/// </summary>
// Requires a real LIFX cloud AppToken (user secrets / appsettings.json), so it cannot run on CI.
[Trait("Category", "Integration")]
[Collection("Cloud API Tests")]
public class LightsTests(ITestOutputHelper testOutputHelper) : CloudLightStateTest(testOutputHelper)
{
	#region List Operations

	/// <summary>
	/// Performs ListAsync_All_Should_Return_Lights operation.
	/// </summary>
	[Fact]
	public async Task ListAsync_All_Should_Return_Lights()
	{
		// Act
		var lights = await Client.Lights.ListAsync(Selector.All, CancellationToken);

		// Assert
		lights.Should().NotBeNull();
		lights.Should().NotBeEmpty();
		Logger.LogInformation("Found {Count} lights", lights.Count);
	}

	/// <summary>
	/// Performs ListAsync_ByLightId_Should_Return_Single_Light operation.
	/// </summary>
	[Fact]
	public async Task ListAsync_ByLightId_Should_Return_Single_Light()
	{
		// Arrange
		var lightId = TestLight!.Id;

		// Act
		var lights = await Client.Lights.ListAsync(new Selector.LightId(lightId), CancellationToken);

		// Assert
		lights.Should().NotBeNull();
		lights.Should().ContainSingle();
		lights[0].Id.Should().Be(lightId);
	}

	/// <summary>
	/// Performs ListAsync_ByLabel_Should_Return_Matching_Light operation.
	/// </summary>
	[Fact]
	public async Task ListAsync_ByLabel_Should_Return_Matching_Light()
	{
		// Arrange
		var label = TestLight!.Label;

		// Act
		var lights = await Client
			.Lights
			.ListAsync(new Selector.LightLabel(label), CancellationToken);

		// Assert
		lights.Should().NotBeNull();
		lights.Should().NotBeEmpty();
		lights.Should().Contain(l => l.Label == label);
	}

	/// <summary>
	/// Performs ListGroupsAsync_Should_Return_Groups operation.
	/// </summary>
	[Fact]
	public async Task ListGroupsAsync_Should_Return_Groups()
	{
		// Act
		var groups = await Client.Lights.ListGroupsAsync(Selector.All, CancellationToken);

		// Assert
		groups.Should().NotBeNull();
		Logger.LogInformation("Found {Count} groups", groups.Count);
	}

	/// <summary>
	/// Performs ListLocationsAsync_Should_Return_Locations operation.
	/// </summary>
	[Fact]
	public async Task ListLocationsAsync_Should_Return_Locations()
	{
		// Act
		var locations = await Client.Lights.ListLocationsAsync(Selector.All, CancellationToken);

		// Assert
		locations.Should().NotBeNull();
		Logger.LogInformation("Found {Count} locations", locations.Count);
	}

	#endregion

	#region Power Operations - Single Light

	/// <summary>
	/// Applies a state change to the light under test.
	/// </summary>
	/// <remarks>
	/// Every state test below issued the same three-line call against the same selector; only the
	/// request differed, so that is all each test now builds.
	/// </remarks>
	private Task<SuccessResponse> SetTestLightStateAsync(SetStateRequest request)
		=> Client.Lights.SetStateAsync(
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

	/// <summary>
	/// Performs SetState_Power_Single_Light_Should_Succeed operation.
	/// </summary>
	[Theory]
	[InlineData(PowerState.On)]
	[InlineData(PowerState.Off)]
	public async Task SetState_Power_Single_Light_Should_Succeed(PowerState powerState)
	{
		// Arrange
		var request = new SetStateRequest { Power = powerState, Duration = 0.5 };

		// Act
		var result = await SetTestLightStateAsync(request);

		// Assert
		result.Should().NotBeNull();
		Logger.LogInformation("Turned {Power} light: {Label}", powerState, TestLight!.Label);
	}

	/// <summary>
	/// Performs TogglePower_Single_Light_Should_Succeed operation.
	/// </summary>
	[Fact]
	public async Task TogglePower_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new TogglePowerRequest { Duration = 0.5 };

		// Act
		var result = await Client.Lights.TogglePowerAsync(
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	#endregion

	#region Color Operations - Single Light

	/// <summary>
	/// The colour forms the LIFX cloud accepts, with the brightness each case sends (null leaves
	/// brightness out of the request, as the HSBK form carries its own).
	/// </summary>
	public static TheoryData<string, double?> ColorForms => new()
	{
		{ "red", 0.8 },
		{ "rgb:0,255,0", 0.7 },
		{ "hue:240 saturation:1.0 brightness:0.8", null },
		{ "kelvin:3500", 0.9 }
	};

	/// <summary>
	/// Performs SetState_Color_Single_Light_Should_Succeed operation.
	/// </summary>
	[Theory]
	[MemberData(nameof(ColorForms))]
	public async Task SetState_Color_Single_Light_Should_Succeed(string color, double? brightness)
	{
		// Arrange
		var request = new SetStateRequest
		{
			Color = color,
			Brightness = brightness,
			Duration = 1.0
		};

		// Act
		var result = await SetTestLightStateAsync(request);

		// Assert
		result.Should().NotBeNull();
		Logger.LogInformation("Set colour {Color} on {Label}", color, TestLight!.Label);
	}

	#endregion

	#region Group Operations

	/// <summary>
	/// Performs SetState_Group_Should_Affect_Multiple_Lights operation.
	/// </summary>
	[Fact]
	public async Task SetState_Group_Should_Affect_Multiple_Lights()
	{
		// Arrange
		var group = await GetTestGroupAsync();
		var request = new SetStateRequest
		{
			Color = "blue",
			Brightness = 0.6,
			Duration = 1.0
		};

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.GroupId(group.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		Logger.LogInformation("Set state for group {Name}", group.Label);
	}

	/// <summary>
	/// Performs TogglePower_Group_Should_Affect_Multiple_Lights operation.
	/// </summary>
	[Fact]
	public async Task TogglePower_Group_Should_Affect_Multiple_Lights()
	{
		// Arrange
		var group = await GetTestGroupAsync();
		var request = new TogglePowerRequest { Duration = 0.5 };

		// Act
		var result = await Client.Lights.TogglePowerAsync(
			new Selector.GroupId(group.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	#endregion

	#region Advanced Operations

	/// <summary>
	/// Performs SetStatesAsync_Multiple_Lights_Different_Colors_Should_Succeed operation.
	/// </summary>
	[Fact]
	public async Task SetStatesAsync_Multiple_Lights_Different_Colors_Should_Succeed()
	{
		// Arrange
		var request = new SetStatesRequest
		{
			States =
			[
				new StateUpdate
				{
					Selector = $"id:{TestLight!.Id}",
					Color = "red",
					Brightness = 0.7
				}
			],
			Defaults = new StateDefaults
			{
				Duration = 1.0,
				Power = PowerState.On
			}
		};

		// Act
		var result = await Client.Lights.SetStatesAsync(request, CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	/// <summary>
	/// Performs StateDelta_Increase_Brightness_Should_Succeed operation.
	/// </summary>
	[Fact]
	public async Task StateDelta_Increase_Brightness_Should_Succeed()
	{
		// Arrange
		var request = new StateDeltaRequest
		{
			Brightness = 0.1, // Increase by 10%
			Duration = 1.0
		};

		// Act
		var result = await Client.Lights.StateDeltaAsync(
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	#endregion

	#region Color Validation

	/// <summary>
	/// Colour strings the validation endpoint should accept, with the hue each must report when
	/// the form fixes one (null where the form leaves hue to the service).
	/// </summary>
	public static TheoryData<string, int?> ValidatableColors => new()
	{
		{ "red", null },
		{ "rgb:255,0,0", null },
		{ "hue:120 saturation:1.0", 120 }
	};

	/// <summary>
	/// Performs ValidateColor_Should_Return_Valid_Result operation.
	/// </summary>
	[Theory]
	[MemberData(nameof(ValidatableColors))]
	public async Task ValidateColor_Should_Return_Valid_Result(string color, int? expectedHue)
	{
		// Act
		var result = await Client.Color.ValidateColorAsync(color, CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Hue.Should().NotBeNull();

		if (expectedHue is null)
		{
			result.Hue!.Value.Should().BeGreaterThanOrEqualTo(0);
		}
		else
		{
			result.Hue!.Value.Should().Be(expectedHue.Value);
		}
	}

	#endregion
}
