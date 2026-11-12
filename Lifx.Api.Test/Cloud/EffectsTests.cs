using AwesomeAssertions;
using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

/// <summary>
/// Represents the EffectsTests type.
/// </summary>
// Requires a real LIFX cloud AppToken (user secrets / appsettings.json), so it cannot run on CI.
[Trait("Category", "Integration")]
[Collection("Cloud API Tests")]
public class EffectsTests(ITestOutputHelper testOutputHelper) : CloudLightStateTest(testOutputHelper)
{
	/// <summary>
	/// Stops any effect still running before the lights are restored, so a restored state is not
	/// immediately overwritten by an effect that is still animating.
	/// </summary>
	protected override async Task OnDisposingAsync()
		=> await Client.Effects.OffAsync(
			Selector.All,
			new EffectsOffRequest { PowerOff = false },
			CancellationToken);

	private Selector.LightId SingleLight => new(TestLight!.Id);

	private static void AssertAccepted(object? result) => result.Should().NotBeNull();

	#region Single Light Effects

	/// <summary>
	/// Performs BreatheEffect_Single_Light_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task BreatheEffect_Single_Light_Should_Execute()
	{
		var result = await Client.Effects.BreatheAsync(
			SingleLight,
			new BreatheEffectRequest
			{
				Color = "blue",
				Period = 2.0,
				Cycles = 3.0,
				Persist = false,
				PowerOn = true
			},
			CancellationToken);

		AssertAccepted(result);
		Logger.LogInformation("BreatheEffect executed on {Label}", TestLight!.Label);
	}

	/// <summary>
	/// Performs PulseEffect_Single_Light_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task PulseEffect_Single_Light_Should_Execute()
		=> AssertAccepted(await Client.Effects.PulseAsync(
			SingleLight,
			new PulseEffectRequest
			{
				Color = "red",
				Period = 1.0,
				Cycles = 5.0,
				Persist = false,
				PowerOn = true
			},
			CancellationToken));

	/// <summary>
	/// Performs MorphEffect_Single_Light_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task MorphEffect_Single_Light_Should_Execute()
		=> AssertAccepted(await Client.Effects.MorphAsync(
			SingleLight,
			new MorphEffectRequest { Period = 3.0, Duration = 10.0, PowerOn = true },
			CancellationToken));

	/// <summary>
	/// Performs FlameEffect_Single_Light_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task FlameEffect_Single_Light_Should_Execute()
		=> AssertAccepted(await Client.Effects.FlameAsync(
			SingleLight,
			new FlameEffectRequest { Period = 5.0, Duration = 10.0, PowerOn = true },
			CancellationToken));

	#endregion

	#region Group Effects

	/// <summary>
	/// Performs CloudsEffect_Group_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task CloudsEffect_Group_Should_Execute()
	{
		var group = await GetTestGroupAsync();

		var result = await Client.Effects.CloudsAsync(
			new Selector.GroupId(group.Id),
			new CloudsEffectRequest { Duration = 10, PowerOn = true },
			CancellationToken);

		AssertAccepted(result);
		Logger.LogInformation("CloudsEffect executed on group {Name}", group.Label);
	}

	/// <summary>
	/// Performs MoveEffect_Group_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task MoveEffect_Group_Should_Execute()
	{
		var group = await GetTestGroupAsync();

		AssertAccepted(await Client.Effects.MoveAsync(
			new Selector.GroupId(group.Id),
			new MoveEffectRequest { Direction = "forward", Period = 2.0, PowerOn = true },
			CancellationToken));
	}

	#endregion

	#region Environment Effects

	/// <summary>
	/// Performs SunriseEffect_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task SunriseEffect_Should_Execute()
		=> AssertAccepted(await Client.Effects.SunriseAsync(
			SingleLight,
			new SunriseEffectRequest { Duration = 60, PowerOn = true },
			CancellationToken));

	/// <summary>
	/// Performs SunsetEffect_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task SunsetEffect_Should_Execute()
		=> AssertAccepted(await Client.Effects.SunsetAsync(
			SingleLight,
			new SunsetEffectRequest { Duration = 60, PowerOn = true },
			CancellationToken));

	#endregion

	#region Effect Control

	/// <summary>
	/// Performs EffectsOff_Should_Stop_Running_Effects operation.
	/// </summary>
	[Fact]
	public async Task EffectsOff_Should_Stop_Running_Effects()
	{
		// Start an effect first
		await Client.Effects.BreatheAsync(
			SingleLight,
			new BreatheEffectRequest { Color = "blue", Period = 2.0, Cycles = 10.0 },
			CancellationToken);

		// Wait a moment for effect to start
		await Task.Delay(500, TestContext.Current.CancellationToken);

		AssertAccepted(await Client.Effects.OffAsync(
			SingleLight,
			new EffectsOffRequest { PowerOff = false },
			CancellationToken));
	}

	/// <summary>
	/// Performs EffectsOff_WithPowerOff_Should_Turn_Off_Light operation.
	/// </summary>
	[Fact]
	public async Task EffectsOff_WithPowerOff_Should_Turn_Off_Light()
		=> AssertAccepted(await Client.Effects.OffAsync(
			SingleLight,
			new EffectsOffRequest { PowerOff = true },
			CancellationToken));

	#endregion

	#region Clean Cycle

	/// <summary>
	/// Performs Clean_Start_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task Clean_Start_Should_Execute()
	{
		AssertAccepted(await Client.Lights.CleanAsync(
			SingleLight,
			new CleanRequest { Stop = false, Duration = 3600 },
			CancellationToken));
		Logger.LogInformation("Clean cycle started");
	}

	/// <summary>
	/// Performs Clean_Stop_Should_Execute operation.
	/// </summary>
	[Fact]
	public async Task Clean_Stop_Should_Execute()
	{
		AssertAccepted(await Client.Lights.CleanAsync(
			SingleLight,
			new CleanRequest { Stop = true },
			CancellationToken));
		Logger.LogInformation("Clean cycle stopped");
	}

	#endregion
}
