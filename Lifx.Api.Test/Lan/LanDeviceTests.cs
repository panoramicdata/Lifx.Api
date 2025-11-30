using AwesomeAssertions;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;
using Lifx.Api.Test.Collections;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN device operations (power, label, version, firmware)
/// Note: These tests verify the method signatures and basic parameter validation
/// Full integration testing would require actual hardware or advanced mocking
/// </summary>
[Collection("LAN Tests")]
public class LanDeviceTests : IDisposable
{
	private readonly LanTestFixture _fixture;
	private readonly LightBulb _testDevice;

	public LanDeviceTests(LanTestFixture fixture)
	{
		_fixture = fixture;

		// Create a test device
		_testDevice = new LightBulb(
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
	public async Task SetDevicePowerState_Should_Require_Valid_Device()
	{
		// Arrange - Use shared client from fixture
		if (!_fixture.IsLanStarted)
		{
			// Skip if LAN client couldn't start (expected in CI)
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetDevicePowerStateAsync(null!, PowerState.On, CancellationToken.None));
	}

	[Fact]
	public async Task GetDeviceLabel_Should_Require_Valid_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetDeviceLabelAsync(null!, CancellationToken.None));
	}

	[Fact]
	public async Task SetDeviceLabel_Should_Require_Valid_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetDeviceLabelAsync(null!, "Test", CancellationToken.None));
	}

	[Fact]
	public async Task GetDeviceVersion_Should_Require_Valid_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetDeviceVersionAsync(null!, CancellationToken.None));
	}

	[Fact]
	public async Task GetDeviceHostFirmware_Should_Require_Valid_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetDeviceHostFirmwareAsync(null!, CancellationToken.None));
	}

	[Fact]
	public void Device_MacAddress_Should_Be_6_Bytes()
	{
		// Assert
		_testDevice.MacAddress.Should().NotBeNull();
		_testDevice.MacAddress.Length.Should().Be(6);
	}

	[Fact]
	public void Device_Should_Have_Network_Properties()
	{
		// Assert
		_testDevice.HostName.Should().NotBeNullOrEmpty();
		_testDevice.Port.Should().BeGreaterThan(0u);
		_testDevice.Service.Should().BeGreaterThan((byte)0);
	}

	[Fact]
	public void PowerState_On_Should_Have_Correct_Value()
	{
		// Assert
		PowerState.On.ToString().ToLowerInvariant().Should().Be("on");
	}

	[Fact]
	public void PowerState_Off_Should_Have_Correct_Value()
	{
		// Assert
		PowerState.Off.ToString().ToLowerInvariant().Should().Be("off");
	}
}
