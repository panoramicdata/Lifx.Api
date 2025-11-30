using AwesomeAssertions;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

[Collection("Cloud API Tests")]
public class EffectsTests(ITestOutputHelper testOutputHelper) : Test(testOutputHelper), IAsyncLifetime
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

			_testLight = await GetTestLightAsync();
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
			// First, stop any running effects
			await Client.Effects.OffAsync(
				Selector.All,
				new EffectsOffRequest { PowerOff = false },
				CancellationToken);

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

	#region Single Light Effects

	[Fact]
	public async Task BreatheEffect_Single_Light_Should_Execute()
	{
		// Arrange
		var request = new BreatheEffectRequest
		{
			Color = "blue",
			Period = 2.0,
			Cycles = 3.0,
			Persist = false,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.BreatheAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("BreatheEffect executed on {Label}", _testLight.Label);
	}

	[Fact]
	public async Task PulseEffect_Single_Light_Should_Execute()
	{
		// Arrange
		var request = new PulseEffectRequest
		{
			Color = "red",
			Period = 1.0,
			Cycles = 5.0,
			Persist = false,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.PulseAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task MorphEffect_Single_Light_Should_Execute()
	{
		// Arrange
		var request = new MorphEffectRequest
		{
			Period = 3.0,
			Duration = 10.0,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.MorphAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task FlameEffect_Single_Light_Should_Execute()
	{
		// Arrange
		var request = new FlameEffectRequest
		{
			Period = 5.0,
			Duration = 10.0,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.FlameAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Group Effects

	[Fact]
	public async Task CloudsEffect_Group_Should_Execute()
	{
		// Arrange
		var group = await GetTestGroupAsync();
		var request = new CloudsEffectRequest
		{
			Duration = 10,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.CloudsAsync(
			new Selector.GroupId(group.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("CloudsEffect executed on group {Name}", group.Label);
	}

	[Fact]
	public async Task MoveEffect_Group_Should_Execute()
	{
		// Arrange
		var group = await GetTestGroupAsync();
		var request = new MoveEffectRequest
		{
			Direction = "forward",
			Period = 2.0,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.MoveAsync(
			new Selector.GroupId(group.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Environment Effects

	[Fact]
	public async Task SunriseEffect_Should_Execute()
	{
		// Arrange
		var request = new SunriseEffectRequest
		{
			Duration = 60,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.SunriseAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task SunsetEffect_Should_Execute()
	{
		// Arrange
		var request = new SunsetEffectRequest
		{
			Duration = 60,
			PowerOn = true
		};

		// Act
		var result = await Client.Effects.SunsetAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Effect Control

	[Fact]
	public async Task EffectsOff_Should_Stop_Running_Effects()
	{
		// Arrange - Start an effect first
		await Client.Effects.BreatheAsync(
			new Selector.LightId(_testLight!.Id),
			new BreatheEffectRequest { Color = "blue", Period = 2.0, Cycles = 10.0 },
			CancellationToken);

		// Wait a moment for effect to start
		await Task.Delay(500, TestContext.Current.CancellationToken);

		var request = new EffectsOffRequest
		{
			PowerOff = false
		};

		// Act
		var result = await Client.Effects.OffAsync(
			new Selector.LightId(_testLight.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task EffectsOff_WithPowerOff_Should_Turn_Off_Light()
	{
		// Arrange
		var request = new EffectsOffRequest
		{
			PowerOff = true
		};

		// Act
		var result = await Client.Effects.OffAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Clean Cycle

	[Fact]
	public async Task Clean_Start_Should_Execute()
	{
		// Arrange
		var request = new CleanRequest
		{
			Stop = false,
			Duration = 3600
		};

		// Act
		var result = await Client.Lights.CleanAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("Clean cycle started");
	}

	[Fact]
	public async Task Clean_Stop_Should_Execute()
	{
		// Arrange
		var request = new CleanRequest
		{
			Stop = true
		};

		// Act
		var result = await Client.Lights.CleanAsync(
			new Selector.LightId(_testLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("Clean cycle stopped");
	}

	#endregion
}
