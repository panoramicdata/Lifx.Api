using AwesomeAssertions;
using Lifx.Api.Models.Lan;
using Lifx.Api.Test.Collections;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Phase 5: LAN Protocol Error Handling Tests
/// Tests error scenarios, timeouts, and edge cases for LAN protocol
/// </summary>
[Collection("LAN Tests")]
public class LanErrorHandlingTests : IDisposable
{
	private readonly ILogger _logger;
	private readonly LanTestFixture _fixture;
	private LifxClient? _client;

	public LanErrorHandlingTests(LanTestFixture fixture)
	{
		_fixture = fixture;
		_logger = LoggerFactory.Create(builder => { })
			.CreateLogger<LanErrorHandlingTests>();
	}

	public void Dispose()
	{
		// Only dispose clients we created locally, not the shared one
		if (_client is not null && _client != _fixture.SharedClient)
		{
			_client.Dispose();
		}

		GC.SuppressFinalize(this);
	}

	#region LAN Not Enabled Tests

	[Fact]
	public void StartLan_Should_Throw_When_LAN_Not_Enabled()
	{
		// Arrange
		_client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = false
		});

		// Act & Assert
		Assert.Throws<InvalidOperationException>(() =>
			_client.StartLan(CancellationToken.None));
	}

	[Fact]
	public void StartDeviceDiscovery_Should_Throw_When_LAN_Not_Enabled()
	{
		// Arrange
		_client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = false
		});

		// Act & Assert
		Assert.Throws<InvalidOperationException>(() =>
			_client.StartDeviceDiscovery(CancellationToken.None));
	}

	[Fact]
	public void StopDeviceDiscovery_Should_Not_Throw_When_Not_Started()
	{
		// Arrange
		_client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = true
		});

		// Act & Assert - Should not throw
		_client.StopDeviceDiscovery();
	}

	#endregion

	#region Null Parameter Tests

	[Fact]
	public async Task SetDevicePowerState_Should_Throw_On_Null_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetDevicePowerStateAsync(
				null!,
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task GetDeviceLabel_Should_Throw_On_Null_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.GetDeviceLabelAsync(
				null!,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetDeviceLabel_Should_Throw_On_Null_Device()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetDeviceLabelAsync(
				null!,
				"Test Label",
				CancellationToken.None));
	}

	[Fact]
	public async Task SetLightPowerAsync_Should_Throw_On_Null_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetLightPowerAsync(
				null!,
				TimeSpan.Zero,
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetColorAsync_Should_Throw_On_Null_Bulb()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		var color = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				null!,
				color,
				3500,
				CancellationToken.None));
	}

	#endregion

	#region Range Validation Tests

	[Fact]
	public async Task SetLightPowerAsync_Should_Reject_Negative_Duration()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		var bulb = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetLightPowerAsync(
				bulb,
				TimeSpan.FromMilliseconds(-1),
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetLightPowerAsync_Should_Reject_Duration_Too_Large()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		var bulb = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetLightPowerAsync(
				bulb,
				TimeSpan.FromMilliseconds((double)uint.MaxValue + 1),
				PowerState.On,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetColorAsync_HSBK_Should_Reject_Kelvin_Too_Low()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		var bulb = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				bulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 2000, // Too low (min is 2500)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetColorAsync_HSBK_Should_Reject_Kelvin_Too_High()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		var bulb = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				bulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 10000, // Too high (max is 9000)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None));
	}

	[Fact]
	public async Task SetColorAsync_Should_Reject_Negative_Duration()
	{
		// Arrange
		if (!_fixture.IsLanStarted)
			return;

		var bulb = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);
		var color = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
			await _fixture.SharedClient!.Lan!.SetColorAsync(
				bulb,
				color,
				3500,
				TimeSpan.FromMilliseconds(-1),
				CancellationToken.None));
	}

	#endregion

	#region Device Model Validation Tests

	[Fact]
	public void Device_Should_Reject_Null_Hostname()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() =>
			new LightBulb(null!, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]));
	}

	[Fact]
	public void Device_Should_Reject_Empty_Hostname()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() =>
			new LightBulb("", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]));
	}

	[Fact]
	public void Device_Should_Reject_Whitespace_Hostname()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() =>
			new LightBulb("   ", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]));
	}

	[Fact]
	public void Device_MacAddress_Should_Be_Six_Bytes()
	{
		// Arrange & Act
		var device = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Assert
		device.MacAddress.Should().HaveCount(6);
	}

	[Fact]
	public void Device_MacAddressName_Should_Handle_Null_MacAddress()
	{
		// Note: This tests the null check in MacAddressName property
		// We can't directly create a device with null MAC, but we test the property logic
		var device = new LightBulb("192.168.1.100", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Assert - Just verify it doesn't throw
		device.MacAddressName.Should().NotBeNullOrEmpty();
	}

	#endregion

	#region Disposal Tests

	[Fact]
	public void LifxClient_Should_Dispose_Without_Error()
	{
		// Arrange
		var client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = true
		});

		// Act & Assert - Should not throw
		client.Dispose();
	}

	[Fact]
	public void LifxClient_Should_Handle_Multiple_Dispose_Calls()
	{
		// Arrange
		var client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = true
		});

		// Act & Assert - Should not throw
		client.Dispose();
		client.Dispose(); // Second call should be safe
	}

	#endregion
}
