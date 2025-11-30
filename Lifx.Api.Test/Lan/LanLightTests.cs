using AwesomeAssertions;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;
using Lifx.Api.Test.Collections;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN light operations (color, power, state, infrared)
/// Note: These tests verify method signatures and basic validation
/// </summary>
[Collection("LAN Tests")]
public class LanLightTests : IDisposable
{
	private readonly LanTestFixture _fixture;
	private readonly LightBulb _testBulb;

	public LanLightTests(LanTestFixture fixture)
	{
		_fixture = fixture;

		// Create a test light bulb
		_testBulb = new LightBulb(
			"192.168.1.100",
			[0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01],
			service: 1,
			port: 56700);
	}

	public void Dispose()
	{
		// Don't dispose the shared client - the fixture handles that
		GC.SuppressFinalize(this);
	}

	[Fact]
	public async Task SetLightPower_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetLightPowerAsync(
				null!,
				TimeSpan.Zero,
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetLightPower_Should_Validate_Transition_Duration()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert - Negative duration
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetLightPowerAsync(
				_testBulb,
				TimeSpan.FromMilliseconds(-1),
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetLightPower_Should_Validate_Max_Duration()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert - Duration too large
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetLightPowerAsync(
				_testBulb,
				TimeSpan.FromMilliseconds((double)uint.MaxValue + 1),
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task GetLightPower_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetLightPowerAsync(null!, CancellationToken.None));
	}

	[Fact]
	public async Task SetColor_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		var redColor = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				null!,
				redColor,
				3500,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetColor_HSBK_Should_Validate_Kelvin_Range()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert - Kelvin too low
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				_testBulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 2000, // Too low (min is 2500)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None));

		// Act & Assert - Kelvin too high
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				_testBulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 10000, // Too high (max is 9000)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetColor_Should_Validate_Transition_Duration()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		var redColor = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert - Negative duration
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				_testBulb,
				redColor,
				3500,
				TimeSpan.FromMilliseconds(-1),
				CancellationToken.None));
	}

	[Fact]
	public async Task GetLightState_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetLightStateAsync(null!, CancellationToken.None));
	}

	[Fact]
	public async Task GetInfrared_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetInfraredAsync(null!, CancellationToken.None));
	}

	[Fact]
	public async Task SetInfrared_Should_Require_Valid_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetInfraredAsync(null!, 32768, CancellationToken.None));
	}

	[Fact]
	public void LightBulb_Should_Be_Device()
	{
		// Assert
		_testBulb.Should().BeOfType<LightBulb>();
		(_testBulb is Device).Should().BeTrue();
	}

	[Fact]
	public void Color_RGB_Values_Should_Be_In_Range()
	{
		// Arrange
		var red = new Color { R = 255, G = 0, B = 0 };
		var green = new Color { R = 0, G = 255, B = 0 };
		var blue = new Color { R = 0, G = 0, B = 255 };

		// Assert
		red.R.Should().Be(255);
		red.G.Should().Be(0);
		red.B.Should().Be(0);

		green.R.Should().Be(0);
		green.G.Should().Be(255);
		green.B.Should().Be(0);

		blue.R.Should().Be(0);
		blue.G.Should().Be(0);
		blue.B.Should().Be(255);
	}
}
