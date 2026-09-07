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

	#region Single Light Effects

	/// <summary>
	/// Performs BreatheEffect_Single_Light_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("BreatheEffect executed on {Label}", TestLight.Label);
	}

	/// <summary>
	/// Performs PulseEffect_Single_Light_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	/// <summary>
	/// Performs MorphEffect_Single_Light_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	/// <summary>
	/// Performs FlameEffect_Single_Light_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Group Effects

	/// <summary>
	/// Performs CloudsEffect_Group_Should_Execute operation.
	/// </summary>
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

	/// <summary>
	/// Performs MoveEffect_Group_Should_Execute operation.
	/// </summary>
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

	/// <summary>
	/// Performs SunriseEffect_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	/// <summary>
	/// Performs SunsetEffect_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Effect Control

	/// <summary>
	/// Performs EffectsOff_Should_Stop_Running_Effects operation.
	/// </summary>
	[Fact]
	public async Task EffectsOff_Should_Stop_Running_Effects()
	{
		// Arrange - Start an effect first
		await Client.Effects.BreatheAsync(
			new Selector.LightId(TestLight!.Id),
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
			new Selector.LightId(TestLight.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	/// <summary>
	/// Performs EffectsOff_WithPowerOff_Should_Turn_Off_Light operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
	}

	#endregion

	#region Clean Cycle

	/// <summary>
	/// Performs Clean_Start_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("Clean cycle started");
	}

	/// <summary>
	/// Performs Clean_Stop_Should_Execute operation.
	/// </summary>
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
			new Selector.LightId(TestLight!.Id),
			request,
			CancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().NotBeNull();
		Logger.LogInformation("Clean cycle stopped");
	}

	#endregion
}
