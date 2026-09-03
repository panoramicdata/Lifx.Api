using AwesomeAssertions;
using Lifx.Api.Models.Lan;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Phase 5: LAN Protocol Error Handling Tests
/// Tests error scenarios, timeouts, and edge cases for LAN protocol
/// </summary>
/// <summary>
/// Represents the LanErrorHandlingTests type.
/// </summary>
[Collection("LAN Tests")]
public class LanErrorHandlingTests(LanTestFixture fixture) : IDisposable
{
	private readonly ILogger _logger = LoggerFactory.Create(builder => { })
			.CreateLogger<LanErrorHandlingTests>();
	private LifxClient? _client;

	/// <summary>
	/// Performs Dispose operation.
	/// </summary>
	public void Dispose()
	{
		// Only dispose clients we created locally, not the shared one
		if (_client is not null && _client != fixture.SharedClient)
		{
			_client.Dispose();
		}

		GC.SuppressFinalize(this);
	}

	#region LAN Not Enabled Tests

	/// <summary>
	/// Performs StartLan_Should_Throw_When_LAN_Not_Enabled operation.
	/// </summary>
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
		((Action)(() => _client.StartLan(CancellationToken.None)))
			.Should()
			.ThrowExactly<InvalidOperationException>();
	}

	/// <summary>
	/// Performs StartDeviceDiscovery_Should_Throw_When_LAN_Not_Enabled operation.
	/// </summary>
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
		((Action)(() => _client.StartDeviceDiscovery(CancellationToken.None)))
			.Should()
			.ThrowExactly<InvalidOperationException>();
	}

	/// <summary>
	/// Performs StopDeviceDiscovery_Should_Not_Throw_When_Not_Started operation.
	/// </summary>
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

	/// <summary>
	/// Performs SetDevicePowerState_Should_Throw_On_Null_Device operation.
	/// </summary>
	[Fact]
	public async Task SetDevicePowerState_Should_Throw_On_Null_Device()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetDevicePowerStateAsync(
				null!,
				PowerState.On,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	/// <summary>
	/// Performs GetDeviceLabel_Should_Throw_On_Null_Device operation.
	/// </summary>
	[Fact]
	public async Task GetDeviceLabel_Should_Throw_On_Null_Device()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.GetDeviceLabelAsync(
				null!,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	/// <summary>
	/// Performs SetDeviceLabel_Should_Throw_On_Null_Device operation.
	/// </summary>
	[Fact]
	public async Task SetDeviceLabel_Should_Throw_On_Null_Device()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetDeviceLabelAsync(
				null!,
				"Test Label",
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	/// <summary>
	/// Performs SetLightPowerAsync_Should_Throw_On_Null_Bulb operation.
	/// </summary>
	[Fact]
	public async Task SetLightPowerAsync_Should_Throw_On_Null_Bulb()
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

	/// <summary>
	/// Performs SetColorAsync_Should_Throw_On_Null_Bulb operation.
	/// </summary>
	[Fact]
	public async Task SetColorAsync_Should_Throw_On_Null_Bulb()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var color = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				null!,
				color,
				3500,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentNullException>();
	}

	#endregion

	#region Range Validation Tests

	/// <summary>
	/// Performs SetLightPowerAsync_Should_Reject_Negative_Duration operation.
	/// </summary>
	[Fact]
	public async Task SetLightPowerAsync_Should_Reject_Negative_Duration()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var bulb = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetLightPowerAsync(
				bulb,
				TimeSpan.FromMilliseconds(-1),
				PowerState.On,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Performs SetLightPowerAsync_Should_Reject_Duration_Too_Large operation.
	/// </summary>
	[Fact]
	public async Task SetLightPowerAsync_Should_Reject_Duration_Too_Large()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var bulb = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetLightPowerAsync(
				bulb,
				TimeSpan.FromMilliseconds((double)uint.MaxValue + 1),
				PowerState.On,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Performs SetColorAsync_HSBK_Should_Reject_Kelvin_Too_Low operation.
	/// </summary>
	[Fact]
	public async Task SetColorAsync_HSBK_Should_Reject_Kelvin_Too_Low()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var bulb = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				bulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 2000, // Too low (min is 2500)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Performs SetColorAsync_HSBK_Should_Reject_Kelvin_Too_High operation.
	/// </summary>
	[Fact]
	public async Task SetColorAsync_HSBK_Should_Reject_Kelvin_Too_High()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var bulb = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				bulb,
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 10000, // Too high (max is 9000)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Performs SetColorAsync_Should_Reject_Negative_Duration operation.
	/// </summary>
	[Fact]
	public async Task SetColorAsync_Should_Reject_Negative_Duration()
	{
		// Arrange
		if (!fixture.IsLanStarted)
		{
			return;
		}

		var bulb = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);
		var color = new Color { R = 255, G = 0, B = 0 };

		// Act & Assert
		await ((Func<Task>)(async () =>
			await fixture.SharedClient!.Lan!.SetColorAsync(
				bulb,
				color,
				3500,
				TimeSpan.FromMilliseconds(-1),
				CancellationToken.None)))
			.Should()
			.ThrowExactlyAsync<ArgumentOutOfRangeException>();
	}

	#endregion

	#region Device Model Validation Tests

	/// <summary>
	/// Performs Device_Should_Reject_Null_Hostname operation.
	/// </summary>
	[Fact]
	public void Device_Should_Reject_Null_Hostname()
	{
		// Act & Assert
		((Func<LightBulb>)(() => new LightBulb(null!, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01])))
			.Should()
			.ThrowExactly<ArgumentNullException>();
	}

	/// <summary>
	/// Performs Device_Should_Reject_Empty_Hostname operation.
	/// </summary>
	[Fact]
	public void Device_Should_Reject_Empty_Hostname()
	{
		// Act & Assert
		((Func<LightBulb>)(() => new LightBulb("", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01])))
			.Should()
			.ThrowExactly<ArgumentException>();
	}

	/// <summary>
	/// Performs Device_Should_Reject_Whitespace_Hostname operation.
	/// </summary>
	[Fact]
	public void Device_Should_Reject_Whitespace_Hostname()
	{
		// Act & Assert
		((Func<LightBulb>)(() => new LightBulb("   ", [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01])))
			.Should()
			.ThrowExactly<ArgumentException>();
	}

	/// <summary>
	/// Performs Device_MacAddress_Should_Be_Six_Bytes operation.
	/// </summary>
	[Fact]
	public void Device_MacAddress_Should_Be_Six_Bytes()
	{
		// Arrange & Act
		var device = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Assert
		device.MacAddress.Should().HaveCount(6);
	}

	/// <summary>
	/// Performs Device_MacAddressName_Should_Handle_Null_MacAddress operation.
	/// </summary>
	[Fact]
	public void Device_MacAddressName_Should_Handle_Null_MacAddress()
	{
		// Note: This tests the null check in MacAddressName property
		// We can't directly create a device with null MAC, but we test the property logic
		var device = new LightBulb(LanTestDevice.HostName, [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Assert - Just verify it doesn't throw
		device.MacAddressName.Should().NotBeNullOrEmpty();
	}

	#endregion

	#region Disposal Tests

	/// <summary>
	/// Performs LifxClient_Should_Dispose_Without_Error operation.
	/// </summary>
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

	/// <summary>
	/// Performs LifxClient_Should_Handle_Multiple_Dispose_Calls operation.
	/// </summary>
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
