using AwesomeAssertions;
using Lifx.Api.Lan;
using Lifx.Api.Models.Lan;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN light operations (color, power, state, infrared)
/// Note: These tests verify method signatures and basic validation
/// </summary>
/// <summary>
/// Represents the LanLightTests type.
/// </summary>
[Collection("LAN Tests")]
public class LanLightTests(LanTestFixture fixture) : IDisposable
{
	private static readonly Color RedColor = new() { R = 255, G = 0, B = 0 };

	private readonly LightBulb _testBulb = new(
			LanTestDevice.HostName,
			LanTestDevice.MacAddress,
			service: 1,
			port: 56700);

	/// <summary>
	/// Performs Dispose operation.
	/// </summary>
	public void Dispose()
	{
		// Don't dispose the shared client - the fixture handles that
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Runs the call against the shared LAN client and asserts the exact exception type.
	/// Does nothing when the LAN client was not started.
	/// </summary>
	private async Task AssertThrowsAsync<TException>(Func<LifxLanClient, Task> call)
		where TException : Exception
	{
		if (!fixture.IsLanStarted)
		{
			return;
		}

		await ((Func<Task>)(() => call(fixture.SharedClient!.Lan!)))
			.Should()
			.ThrowExactlyAsync<TException>();
	}

	private Task SetHsbkAsync(LifxLanClient lan, ushort kelvin)
		=> lan.SetColorAsync(
			_testBulb,
			hue: 0,
			saturation: 65535,
			brightness: 65535,
			kelvin: kelvin,
			transitionDuration: TimeSpan.Zero,
			CancellationToken.None);

	/// <summary>
	/// Performs SetLightPower_Should_Require_Valid_Bulb operation.
	/// </summary>
	[Fact]
	public Task SetLightPower_Should_Require_Valid_Bulb()
		=> AssertThrowsAsync<ArgumentNullException>(lan =>
			lan.SetLightPowerAsync(null!, TimeSpan.Zero, PowerState.On, CancellationToken.None));

	/// <summary>
	/// Performs SetLightPower_Should_Validate_Transition_Duration operation.
	/// </summary>
	[Fact]
	public Task SetLightPower_Should_Validate_Transition_Duration()
		=> AssertThrowsAsync<ArgumentOutOfRangeException>(lan =>
			lan.SetLightPowerAsync(_testBulb, TimeSpan.FromMilliseconds(-1), PowerState.On, CancellationToken.None));

	/// <summary>
	/// Performs SetLightPower_Should_Validate_Max_Duration operation.
	/// </summary>
	[Fact]
	public Task SetLightPower_Should_Validate_Max_Duration()
		=> AssertThrowsAsync<ArgumentOutOfRangeException>(lan =>
			lan.SetLightPowerAsync(
				_testBulb,
				TimeSpan.FromMilliseconds((double)uint.MaxValue + 1),
				PowerState.On,
				CancellationToken.None));

	/// <summary>
	/// Performs GetLightPower_Should_Require_Valid_Bulb operation.
	/// </summary>
	[Fact]
	public Task GetLightPower_Should_Require_Valid_Bulb()
		=> AssertThrowsAsync<ArgumentNullException>(lan =>
			lan.GetLightPowerAsync(null!, CancellationToken.None));

	/// <summary>
	/// Performs SetColor_Should_Require_Valid_Bulb operation.
	/// </summary>
	[Fact]
	public Task SetColor_Should_Require_Valid_Bulb()
		=> AssertThrowsAsync<ArgumentNullException>(lan =>
			lan.SetColorAsync(null!, RedColor, 3500, CancellationToken.None));

	/// <summary>
	/// Performs SetColor_HSBK_Should_Validate_Kelvin_Range operation.
	/// </summary>
	[Fact]
	public async Task SetColor_HSBK_Should_Validate_Kelvin_Range()
	{
		// Kelvin too low (min is 2500)
		await AssertThrowsAsync<ArgumentOutOfRangeException>(lan => SetHsbkAsync(lan, 2000));

		// Kelvin too high (max is 9000)
		await AssertThrowsAsync<ArgumentOutOfRangeException>(lan => SetHsbkAsync(lan, 10000));
	}

	/// <summary>
	/// Performs SetColor_Should_Validate_Transition_Duration operation.
	/// </summary>
	[Fact]
	public Task SetColor_Should_Validate_Transition_Duration()
		=> AssertThrowsAsync<ArgumentOutOfRangeException>(lan =>
			lan.SetColorAsync(
				_testBulb,
				RedColor,
				3500,
				TimeSpan.FromMilliseconds(-1),
				CancellationToken.None));

	/// <summary>
	/// Performs GetLightState_Should_Require_Valid_Bulb operation.
	/// </summary>
	[Fact]
	public Task GetLightState_Should_Require_Valid_Bulb()
		=> AssertThrowsAsync<ArgumentNullException>(lan =>
			lan.GetLightStateAsync(null!, CancellationToken.None));

	/// <summary>
	/// Performs GetInfrared_Should_Require_Valid_Bulb operation.
	/// </summary>
	[Fact]
	public Task GetInfrared_Should_Require_Valid_Bulb()
		=> AssertThrowsAsync<ArgumentNullException>(lan =>
			lan.GetInfraredAsync(null!, CancellationToken.None));

	/// <summary>
	/// Performs SetInfrared_Should_Require_Valid_Device operation.
	/// </summary>
	[Fact]
	public Task SetInfrared_Should_Require_Valid_Device()
		=> AssertThrowsAsync<ArgumentNullException>(lan =>
			lan.SetInfraredAsync(null!, 32768, CancellationToken.None));

	/// <summary>
	/// Performs LightBulb_Should_Be_Device operation.
	/// </summary>
	[Fact]
	public void LightBulb_Should_Be_Device()
	{
		// Assert
		_testBulb.Should().BeOfType<LightBulb>();
		_testBulb.Should().BeAssignableTo<Device>();
	}

	/// <summary>
	/// Performs Color_RGB_Values_Should_Be_In_Range operation.
	/// </summary>
	[Fact]
	public void Color_RGB_Values_Should_Be_In_Range()
	{
		// Arrange
		var green = new Color { R = 0, G = 255, B = 0 };
		var blue = new Color { R = 0, G = 0, B = 255 };

		// Assert
		AssertRgb(RedColor, 255, 0, 0);
		AssertRgb(green, 0, 255, 0);
		AssertRgb(blue, 0, 0, 255);
	}

	private static void AssertRgb(Color color, byte r, byte g, byte b)
	{
		color.R.Should().Be(r);
		color.G.Should().Be(g);
		color.B.Should().Be(b);
	}

	/// <summary>
	/// The HSBK overload assembles its packet payload from a params object array. A
	/// CancellationToken that leaks into that array is rejected as an unsupported payload
	/// type before the socket is ever touched, which broke the call for every caller.
	/// </summary>
	[Fact]
	public async Task SetColorAsync_HSBK_Should_Not_Reject_Its_Own_Arguments()
	{
		// Arrange - LAN is enabled but never started, so a correctly assembled call must
		// fail on the missing socket rather than on the payload it built.
		using var client = new LifxClient(new LifxClientOptions { IsLanEnabled = true });

		// Act & Assert
		await ((Func<Task>)(() => client.Lan!.SetColorAsync(
				_testBulb,
				hue: 0,
				saturation: 0,
				brightness: 65535,
				kelvin: 3500,
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<InvalidOperationException>();
	}
}
