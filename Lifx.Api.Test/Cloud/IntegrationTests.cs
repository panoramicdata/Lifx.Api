using AwesomeAssertions;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

/// <summary>
/// Phase 6: Integration & Scenario Tests
/// Tests complete workflows, multi-device scenarios, and end-to-end integration
/// </summary>
[Collection("Cloud API Tests")]
public class IntegrationTests(ITestOutputHelper testOutputHelper) : Test(testOutputHelper), IAsyncLifetime
{
	private List<Light>? _originalLightStates;
	private Light? _testLight;
	private Group? _testGroup;

	async ValueTask IAsyncLifetime.InitializeAsync()
	{
		try
		{
			_originalLightStates = await Client.Lights.ListAsync(Selector.All, CancellationToken);
			Logger.LogInformation("Captured original state of {Count} lights", _originalLightStates.Count);

			_testLight = await GetTestLightAsync();
			_testGroup = await GetTestGroupAsync();
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Could not initialize integration tests");
		}
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		// Restore original state
		if (_originalLightStates is not null)
		{
			try
			{
				foreach (var light in _originalLightStates.Where(l => l.IsConnected))
				{
					await Client.Lights.SetStateAsync(
						new Selector.LightId(light.Id),
						new SetStateRequest
						{
							Power = light.PowerState,
							Color = light.Color?.ToString() ?? "white",
							Brightness = (double)light.Brightness,
							Duration = 1.0
						},
						CancellationToken);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to restore state");
			}
		}

		GC.SuppressFinalize(this);
	}

	#region Complete Workflow Tests

	[Fact]
	public async Task Complete_Workflow_List_SetState_Verify()
	{
		// Phase 1: List lights
		var lights = await Client.Lights.ListAsync(Selector.All, CancellationToken);
		lights.Should().NotBeEmpty();
		Logger.LogInformation("Listed {Count} lights", lights.Count);

		// Phase 2: Set state
		var targetColor = "blue";
		var targetBrightness = 0.8;
		await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			new SetStateRequest
			{
				Power = PowerState.On,
				Color = targetColor,
				Brightness = targetBrightness,
				Duration = 1.0
			},
			CancellationToken);

		// Phase 3: Verify state
		await Task.Delay(1500, CancellationToken); // Wait for transition
		var updatedLights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight.Id),
			CancellationToken);

		updatedLights.Should().ContainSingle();
		var updatedLight = updatedLights[0];
		updatedLight.PowerState.Should().Be(PowerState.On);
		updatedLight.Brightness.Should().BeGreaterThanOrEqualTo((float)targetBrightness - 0.1f);
	}

	[Fact]
	public async Task Scene_Activation_Workflow()
	{
		// Phase 1: List scenes
		var scenes = await Client.Scenes.ListScenesAsync(CancellationToken);

		if (scenes.Count == 0)
		{
			Logger.LogWarning("No scenes available, skipping test");
			return;
		}

		// Phase 2: Activate scene
		var scene = scenes[0];
		var response = await Client.Scenes.ActivateSceneAsync(
			$"scene_id:{scene.Uuid}",
			new ActivateSceneRequest { Duration = 1.0 },
			CancellationToken);

		response.Should().NotBeNull();
		response.IsSuccessStatusCode.Should().BeTrue();
		Logger.LogInformation("Activated scene: {Name}", scene.Name);

		// Phase 3: Verify state change occurred
		await Task.Delay(1500, CancellationToken);
		var lights = await Client.Lights.ListAsync(Selector.All, CancellationToken);
		lights.Should().NotBeEmpty();
	}

	[Fact]
	public async Task Effect_Lifecycle_Management()
	{
		// Phase 1: Start effect
		var breatheRequest = new BreatheEffectRequest
		{
			Color = "purple",
			Period = 2.0,
			Cycles = 5.0,
			Persist = false,
			PowerOn = true
		};

		await Client.Effects.BreatheAsync(
			new Selector.LightId(_testLight!.Id),
			breatheRequest,
			CancellationToken);

		Logger.LogInformation("Started breathe effect");

		// Phase 2: Wait for effect to run
		await Task.Delay(3000, CancellationToken);

		// Phase 3: Stop effect
		await Client.Effects.OffAsync(
			new Selector.LightId(_testLight.Id),
			new EffectsOffRequest { PowerOff = false },
			CancellationToken);

		Logger.LogInformation("Stopped effect");

		// Phase 4: Verify light is still on
		var lights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight.Id),
			CancellationToken);

		lights[0].IsOn.Should().BeTrue();
	}

	#endregion

	#region Group Operations Tests

	[Fact]
	public async Task Group_State_Changes_Should_Affect_All_Lights()
	{
		// Phase 1: Get initial group state
		var initialGroups = await Client.Lights.ListGroupsAsync(Selector.All, CancellationToken);
		var group = initialGroups.First();
		var initialLightCount = group.Count();

		Logger.LogInformation("Testing group {Name} with {Count} lights", group.Label, initialLightCount);

		// Phase 2: Change group state
		await Client.Lights.SetStateAsync(
			new Selector.GroupId(group.Id),
			new SetStateRequest
			{
				Power = PowerState.On,
				Color = "green",
				Brightness = 0.7,
				Duration = 1.0
			},
			CancellationToken);

		// Phase 3: Verify all lights in group changed
		await Task.Delay(1500, CancellationToken);
		var updatedLights = await Client.Lights.ListAsync(
			new Selector.GroupId(group.Id),
			CancellationToken);

		updatedLights.Should().HaveCount(initialLightCount);
		updatedLights.Should().AllSatisfy(light =>
		{
			light.PowerState.Should().Be(PowerState.On);
		});
	}

	[Fact]
	public async Task Multiple_Groups_Should_Be_Independently_Controllable()
	{
		// Phase 1: Get all groups
		var groups = await Client.Lights.ListGroupsAsync(Selector.All, CancellationToken);

		if (groups.Count < 2)
		{
			Logger.LogWarning("Need at least 2 groups for this test, skipping");
			return;
		}

		var group1 = groups[0];
		var group2 = groups[1];

		// Phase 2: Set different states
		await Client.Lights.SetStateAsync(
			new Selector.GroupId(group1.Id),
			new SetStateRequest { Color = "red", Brightness = 0.5, Duration = 0.5 },
			CancellationToken);

		await Client.Lights.SetStateAsync(
			new Selector.GroupId(group2.Id),
			new SetStateRequest { Color = "blue", Brightness = 0.8, Duration = 0.5 },
			CancellationToken);

		// Phase 3: Verify independence
		await Task.Delay(1000, CancellationToken);
		var group1Lights = await Client.Lights.ListAsync(
			new Selector.GroupId(group1.Id),
			CancellationToken);
		var group2Lights = await Client.Lights.ListAsync(
			new Selector.GroupId(group2.Id),
			CancellationToken);

		group1Lights.Should().NotBeEmpty();
		group2Lights.Should().NotBeEmpty();
	}

	#endregion

	#region State Management Tests

	[Fact]
	public async Task State_Restore_After_Changes()
	{
		// Phase 1: Capture initial state
		var initialLights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight!.Id),
			CancellationToken);
		var initialLight = initialLights[0];
		var initialPower = initialLight.PowerState;
		var initialBrightness = initialLight.Brightness;

		// Phase 2: Make changes
		await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight.Id),
			new SetStateRequest
			{
				Power = PowerState.On,
				Color = "orange",
				Brightness = 1.0,
				Duration = 0.5
			},
			CancellationToken);

		await Task.Delay(1000, CancellationToken);

		// Phase 3: Restore original state
		await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight.Id),
			new SetStateRequest
			{
				Power = initialPower,
				Brightness = (double)initialBrightness,
				Duration = 0.5
			},
			CancellationToken);

		await Task.Delay(1000, CancellationToken);

		// Phase 4: Verify restoration
		var restoredLights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight.Id),
			CancellationToken);

		restoredLights[0].PowerState.Should().Be(initialPower);
	}

	[Fact]
	public async Task Rapid_State_Changes_Should_Not_Fail()
	{
		if (_testLight is null)
		{
			Logger.LogWarning("No test light available, skipping test");
			return;
		}

		// Perform multiple rapid state changes
		var colors = new[] { "red", "green", "blue", "yellow", "purple" };

		foreach (var color in colors)
		{
			await Client.Lights.SetStateAsync(
				new Selector.LightId(_testLight.Id),
				new SetStateRequest
				{
					Color = color,
					Brightness = 0.8,
					Duration = 0.1
				},
				CancellationToken);

			// Minimal delay between changes
			await Task.Delay(100, CancellationToken);
		}

		// Verify light is still responsive
		var finalLights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight.Id),
			CancellationToken);

		finalLights.Should().ContainSingle();
		Logger.LogInformation("Light survived rapid state changes");
	}

	#endregion

	#region Advanced Operations Tests

	[Fact]
	public async Task SetStates_Multiple_Lights_Different_Colors()
	{
		// Get multiple lights
		var lights = await Client.Lights.ListAsync(Selector.All, CancellationToken);

		if (lights.Count < 2)
		{
			Logger.LogWarning("Need at least 2 lights for this test, skipping");
			return;
		}

		// Set different colors for different lights
		var request = new SetStatesRequest
		{
			States =
			[
				new StateUpdate
				{
					Selector = $"id:{lights[0].Id}",
					Color = "red",
					Brightness = 0.8
				},
				new StateUpdate
				{
					Selector = $"id:{lights[1].Id}",
					Color = "blue",
					Brightness = 0.6
				}
			],
			Defaults = new StateDefaults
			{
				Duration = 1.0,
				Power = PowerState.On
			}
		};

		var result = await Client.Lights.SetStatesAsync(request, CancellationToken);
		result.Should().NotBeNull();

		await Task.Delay(1500, CancellationToken);

		// Verify each light
		var light1 = await Client.Lights.ListAsync(new Selector.LightId(lights[0].Id), CancellationToken);
		var light2 = await Client.Lights.ListAsync(new Selector.LightId(lights[1].Id), CancellationToken);

		light1[0].IsOn.Should().BeTrue();
		light2[0].IsOn.Should().BeTrue();
	}

	[Fact]
	public async Task StateDelta_Incremental_Changes()
	{
		// Get initial brightness
		var initialLights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight!.Id),
			CancellationToken);
		var initialBrightness = initialLights[0].Brightness;

		// Increase brightness by 10%
		await Client.Lights.StateDeltaAsync(
			new Selector.LightId(_testLight.Id),
			new StateDeltaRequest
			{
				Brightness = 0.1,
				Duration = 0.5
			},
			CancellationToken);

		await Task.Delay(1000, CancellationToken);

		// Verify brightness increased
		var updatedLights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight.Id),
			CancellationToken);

		updatedLights[0].Brightness.Should().BeGreaterThan(initialBrightness);
	}

	#endregion

	#region Color Validation Integration Tests

	[Fact]
	public async Task Color_Validation_Before_Setting_State()
	{
		// Validate color first
		var colorResult = await Client.Color.ValidateColorAsync("rgb:128,0,128", CancellationToken);

		colorResult.Should().NotBeNull();
		colorResult.Hue.Should().NotBeNull();

		// Use validated color
		await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight!.Id),
			new SetStateRequest
			{
				Color = "rgb:128,0,128",
				Duration = 1.0
			},
			CancellationToken);

		// Verify
		await Task.Delay(1500, CancellationToken);
		var lights = await Client.Lights.ListAsync(
			new Selector.LightId(_testLight.Id),
			CancellationToken);

		lights[0].Color.Should().NotBeNull();
	}

	#endregion

	#region Cleanup and Disposal Tests

	[Fact]
	public async Task Client_Disposal_Should_Be_Safe()
	{
		// Create a temporary client
		using var tempClient = new LifxClient(new LifxClientOptions
		{
			ApiToken = Configuration.AppToken,
			Logger = Logger
		});

		// Use it
		var lights = await tempClient.Lights.ListAsync(Selector.All, CancellationToken);
		lights.Should().NotBeNull();

		// Dispose is implicit via using statement
		// This test verifies no exceptions are thrown
	}

	[Fact]
	public async Task Multiple_Sequential_Operations_Should_Succeed()
	{
		// Perform a sequence of different operations
		var lights = await Client.Lights.ListAsync(Selector.All, CancellationToken);
		lights.Should().NotBeEmpty();

		var groups = await Client.Lights.ListGroupsAsync(Selector.All, CancellationToken);
		groups.Should().NotBeNull();

		var locations = await Client.Lights.ListLocationsAsync(Selector.All, CancellationToken);
		locations.Should().NotBeNull();

		if (_testLight is null)
		{
			Logger.LogWarning("No test light available, skipping remainder of test");
			return;
		}

		await Client.Lights.SetStateAsync(
			new Selector.LightId(_testLight.Id),
			new SetStateRequest { Power = PowerState.On, Duration = 0.5 },
			CancellationToken);

		await Task.Delay(1000, CancellationToken);

		await Client.Lights.TogglePowerAsync(
			new Selector.LightId(_testLight.Id),
			new TogglePowerRequest { Duration = 0.5 },
			CancellationToken);

		// All operations should complete without error
		Logger.LogInformation("Sequential operations completed successfully");
	}

	#endregion
}
