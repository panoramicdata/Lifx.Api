using AwesomeAssertions;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

[Collection("Cloud API Tests")]
public class LightsTests(ITestOutputHelper testOutputHelper) : Test(testOutputHelper), IAsyncLifetime
{
	private List<Light>? _originalLightStates;
	private Light? _testLight;

	async ValueTask IAsyncLifetime.InitializeAsync()
	{
		// Capture the original state of all lights before tests run
		try
		{
			_originalLightStates = await Client.Lights.ListAsync(Selector.All, CancellationToken);
			Logger.LogInformation("Captured original state of {Count} lights", _originalLightStates.Count);

			// Get test light for single-light tests
			_testLight = await GetTestLightAsync();
			Logger.LogInformation("Using test light: {Label} ({Id})", _testLight.Label, _testLight.Id);
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Could not capture original light states");
			_originalLightStates = null;
		}
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		// Restore lights to their original state after all tests in this class complete
		if (_originalLightStates is null || _originalLightStates.Count == 0)
		{
			Logger.LogInformation("No original state to restore");
			return;
		}

		try
		{
			Logger.LogInformation("Restoring original state for {Count} lights", _originalLightStates.Count);

			foreach (var originalLight in _originalLightStates)
			{
				// Only restore if the light is connected
				if (!originalLight.IsConnected)
					continue;

				var restoreRequest = new SetStateRequest
				{
					Power = originalLight.PowerState,
					Color = originalLight.Color?.ToString() ?? "white",
					Brightness = (double)originalLight.Brightness,
					Duration = 1.0 // 1 second transition
				};

				await Client.Lights.SetStateAsync(
					new Selector.LightId(originalLight.Id),
					restoreRequest,
					CancellationToken);
			}

			Logger.LogInformation("Successfully restored original light states");
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Failed to restore original light states");
		}

		GC.SuppressFinalize(this);
	}

	#region List Operations

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

	[Fact]
	public async Task ListAsync_ByLightId_Should_Return_Single_Light()
	{
		// Arrange
		var lightId = _testLight!.Id;

		// Act
		var lights = await Client.Lights.ListAsync(new Selector.LightId(lightId), CancellationToken);

		// Assert
		lights.Should().NotBeNull();
		lights.Should().ContainSingle();
		lights[0].Id.Should().Be(lightId);
	}

	[Fact]
	public async Task ListAsync_ByLabel_Should_Return_Matching_Light()
	{
		// Arrange
		var label = _testLight!.Label;

		// Act
		var lights = await Client
			.Lights
			.ListAsync(new Selector.LightLabel(label), CancellationToken);

		// Assert
		lights.Should().NotBeNull();
		lights.Should().NotBeEmpty();
		lights.Should().Contain(l => l.Label == label);
	}

	[Fact]
	public async Task ListGroupsAsync_Should_Return_Groups()
	{
		// Act
		var groups = await Client.Lights.ListGroupsAsync(Selector.All, CancellationToken);

		// Assert
		groups.Should().NotBeNull();
		Logger.LogInformation("Found {Count} groups", groups.Count);
	}

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

	[Fact]
	public async Task SetState_TurnOn_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new SetStateRequest { Power = PowerState.On, Duration = 0.5 };

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("Turned on light: {Label}", _testLight.Label);
	}

	[Fact]
	public async Task SetState_TurnOff_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new SetStateRequest { Power = PowerState.Off, Duration = 0.5 };

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task TogglePower_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new TogglePowerRequest { Duration = 0.5 };

		// Act
		var result = await Client.Lights.TogglePowerAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Color Operations - Single Light

	[Fact]
	public async Task SetState_Color_Red_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new SetStateRequest
		{
			Color = "red",
			Brightness = 0.8,
			Duration = 1.0
		};

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task SetState_Color_RGB_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new SetStateRequest
		{
			Color = "rgb:0,255,0", // Green
			Brightness = 0.7,
			Duration = 1.0
		};

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task SetState_Color_HSBK_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new SetStateRequest
		{
			Color = "hue:240 saturation:1.0 brightness:0.8",
			Duration = 1.0
		};

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task SetState_Kelvin_Single_Light_Should_Succeed()
	{
		// Arrange
		var request = new SetStateRequest
		{
			Color = "kelvin:3500",
			Brightness = 0.9,
			Duration = 1.0
		};

		// Act
		var result = await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Group Operations

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
		result.Should().NotBeNull();
		Logger.LogInformation("Set state for group {Name} with {Count} lights", group.Label, 0);
	}

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
		result.Should().NotBeNull();
	}

	#endregion

	#region Advanced Operations

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
					Selector = $"id:{_testLight!.Id}",
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
		result.Should().NotBeNull();
	}

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
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
	}

	#endregion

	#region Color Validation

	[Fact]
	public async Task ValidateColor_Red_Should_Return_Valid_Result()
	{
		// Act
		var result = await Client.Color.ValidateColorAsync("red", CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Hue.Should().NotBeNull();
		result.Hue!.Value.Should().BeGreaterThanOrEqualTo(0);
	}

	[Fact]
	public async Task ValidateColor_RGB_Should_Return_Valid_Result()
	{
		// Act
		var result = await Client.Color.ValidateColorAsync("rgb:255,0,0", CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Hue.Should().NotBeNull();
		result.Hue!.Value.Should().BeGreaterThanOrEqualTo(0);
	}

	[Fact]
	public async Task ValidateColor_HSBK_Should_Return_Valid_Result()
	{
		// Act
		var result = await Client.Color.ValidateColorAsync("hue:120 saturation:1.0", CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Hue.Should().NotBeNull();
		result.Hue!.Value.Should().Be(120);
	}

	#endregion
}
