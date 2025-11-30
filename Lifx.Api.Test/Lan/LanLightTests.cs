using AwesomeAssertions;
using Lifx.Api.Models.Lan;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN light operations (color, power, state, infrared)
/// Note: These tests verify method signatures and basic validation
/// </summary>
[Collection("LAN Tests")]
public class LanLightTests(LanTestFixture fixture) : IDisposable
{
	private readonly LightBulb _testBulb = new(
			"192.168.1.100",
			[0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01],
			service: 1,
			port: 56700);

	public void Dispose()
	{
		// Don't dispose the shared client - the fixture handles that
		GC.SuppressFinalize(this);
	}

	[Fact]
	public async Task SetLightPower_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetLightPowerAsync(
				null!,
				TimeSpan.Zero,
				PowerState.On,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task SetLightPower_Should_Validate_Transition_Duration()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert - Negative duration
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetLightPowerAsync(
				_testBulb,
				TimeSpan.FromMilliseconds(-1),
				PowerState.On,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	[Fact]
	public async Task SetLightPower_Should_Validate_Max_Duration()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert - Duration too large
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetLightPowerAsync(
				_testBulb,
				TimeSpan.FromMilliseconds((double)uint.MaxValue + 1),
				PowerState.On,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	[Fact]
	public async Task GetLightPower_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.GetLightPowerAsync(null!, CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task SetColor_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var redColor = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				null!,
				redColor,
				3500,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task SetColor_HSBK_Should_Validate_Kelvin_Range()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert - Kelvin too low
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				_testBulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 2000, // Too low (min is 2500)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();

		// Act & Assert - Kelvin too high
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				_testBulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 10000, // Too high (max is 9000)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	[Fact]
	public async Task SetColor_Should_Validate_Transition_Duration()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var redColor = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert - Negative duration
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				_testBulb,
				redColor,
				3500,
				TimeSpan.FromMilliseconds(-1),
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	[Fact]
	public async Task GetLightState_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.GetLightStateAsync(null!, CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task GetInfrared_Should_Require_Valid_Bulb()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.GetInfraredAsync(null!, CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task SetInfrared_Should_Require_Valid_Device()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetInfraredAsync(null!, 32768, CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	[Fact]
	public void LightBulb_Should_Be_Device()
	{
		// Assert
		_testBulb.Should().BeOfType<LightBulb>();
		_testBulb.Should().BeAssignableTo<Device>();
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
