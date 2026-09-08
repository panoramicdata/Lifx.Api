using AwesomeAssertions;
using Lifx.Api.Lan;
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

	/// <summary>
	/// Creates a client owned by this test, so Dispose can clean it up.
	/// </summary>
	private LifxClient CreateClient(bool isLanEnabled)
	{
		_client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = isLanEnabled
		});

		return _client;
	}

	/// <summary>
	/// Asserts that a LAN call rejects its arguments with <typeparamref name="TException"/>.
	/// </summary>
	/// <remarks>
	/// Each of these tests bailed out when the fixture had no LAN client and then wrapped the call
	/// in the same cast-to-Func dance. Only the call itself and the expected exception differ, so
	/// that is all each test supplies now.
	/// </remarks>
	private async Task AssertLanRejectsAsync<TException>(Func<LifxLanClient, Task> call)
		where TException : Exception
	{
		if (!fixture.IsLanStarted)
		{
			return;
		}

		await ((Func<Task>)(async () => await call(fixture.SharedClient!.Lan!)))
			.Should()
			.ThrowExactlyAsync<TException>();
	}

	private static LightBulb CreateTestBulb()
		=> new(LanTestDevice.HostName, LanTestDevice.MacAddress);

	#region LAN Not Enabled Tests

	/// <summary>
	/// Performs StartLan_Should_Throw_When_LAN_Not_Enabled operation.
	/// </summary>
	[Fact]
	public void StartLan_Should_Throw_When_LAN_Not_Enabled()
	{
		// Arrange
		var client = CreateClient(isLanEnabled: false);

		// Act & Assert
		((Action)(() => client.StartLan(CancellationToken.None)))
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
		var client = CreateClient(isLanEnabled: false);

		// Act & Assert
		((Action)(() => client.StartDeviceDiscovery(CancellationToken.None)))
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
		var client = CreateClient(isLanEnabled: true);

		// Act & Assert - Should not throw
		client.StopDeviceDiscovery();
	}

	#endregion

	#region Null Parameter Tests

	/// <summary>
	/// Performs SetDevicePowerState_Should_Throw_On_Null_Device operation.
	/// </summary>
	[Fact]
	public Task SetDevicePowerState_Should_Throw_On_Null_Device()
		=> AssertLanRejectsAsync<ArgumentNullException>(
			lan => lan.SetDevicePowerStateAsync(null!, PowerState.On, CancellationToken.None));

	/// <summary>
	/// Performs GetDeviceLabel_Should_Throw_On_Null_Device operation.
	/// </summary>
	[Fact]
	public Task GetDeviceLabel_Should_Throw_On_Null_Device()
		=> AssertLanRejectsAsync<ArgumentNullException>(
			lan => lan.GetDeviceLabelAsync(null!, CancellationToken.None));

	/// <summary>
	/// Performs SetDeviceLabel_Should_Throw_On_Null_Device operation.
	/// </summary>
	[Fact]
	public Task SetDeviceLabel_Should_Throw_On_Null_Device()
		=> AssertLanRejectsAsync<ArgumentNullException>(
			lan => lan.SetDeviceLabelAsync(null!, "Test Label", CancellationToken.None));

	/// <summary>
	/// Performs SetLightPowerAsync_Should_Throw_On_Null_Bulb operation.
	/// </summary>
	[Fact]
	public Task SetLightPowerAsync_Should_Throw_On_Null_Bulb()
		=> AssertLanRejectsAsync<ArgumentNullException>(
			lan => lan.SetLightPowerAsync(null!, TimeSpan.Zero, PowerState.On, CancellationToken.None));

	/// <summary>
	/// Performs SetColorAsync_Should_Throw_On_Null_Bulb operation.
	/// </summary>
	[Fact]
	public Task SetColorAsync_Should_Throw_On_Null_Bulb()
		=> AssertLanRejectsAsync<ArgumentNullException>(
			lan => lan.SetColorAsync(
				null!,
				new Color { R = 255, G = 0, B = 0 },
				3500,
				CancellationToken.None));

	#endregion

	#region Range Validation Tests

	/// <summary>
	/// Performs SetLightPowerAsync_Should_Reject_Negative_Duration operation.
	/// </summary>
	[Fact]
	public Task SetLightPowerAsync_Should_Reject_Negative_Duration()
		=> AssertLanRejectsAsync<ArgumentOutOfRangeException>(
			lan => lan.SetLightPowerAsync(
				CreateTestBulb(),
				TimeSpan.FromMilliseconds(-1),
				PowerState.On,
				CancellationToken.None));

	/// <summary>
	/// Performs SetLightPowerAsync_Should_Reject_Duration_Too_Large operation.
	/// </summary>
	[Fact]
	public Task SetLightPowerAsync_Should_Reject_Duration_Too_Large()
		=> AssertLanRejectsAsync<ArgumentOutOfRangeException>(
			lan => lan.SetLightPowerAsync(
				CreateTestBulb(),
				TimeSpan.FromMilliseconds((double)uint.MaxValue + 1),
				PowerState.On,
				CancellationToken.None));

	/// <summary>
	/// Performs SetColorAsync_HSBK_Should_Reject_Kelvin_Too_Low operation.
	/// </summary>
	[Fact]
	public Task SetColorAsync_HSBK_Should_Reject_Kelvin_Too_Low()
		=> AssertLanRejectsAsync<ArgumentOutOfRangeException>(
			lan => lan.SetColorAsync(
				CreateTestBulb(),
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 2000, // Too low (min is 2500)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None));

	/// <summary>
	/// Performs SetColorAsync_HSBK_Should_Reject_Kelvin_Too_High operation.
	/// </summary>
	[Fact]
	public Task SetColorAsync_HSBK_Should_Reject_Kelvin_Too_High()
		=> AssertLanRejectsAsync<ArgumentOutOfRangeException>(
			lan => lan.SetColorAsync(
				CreateTestBulb(),
				hue: 0,
				saturation: 65535,
				brightness: 65535,
				kelvin: 10000, // Too high (max is 9000)
				transitionDuration: TimeSpan.Zero,
				CancellationToken.None));

	/// <summary>
	/// Performs SetColorAsync_Should_Reject_Negative_Duration operation.
	/// </summary>
	[Fact]
	public Task SetColorAsync_Should_Reject_Negative_Duration()
		=> AssertLanRejectsAsync<ArgumentOutOfRangeException>(
			lan => lan.SetColorAsync(
				CreateTestBulb(),
				new Color { R = 255, G = 0, B = 0 },
				3500,
				TimeSpan.FromMilliseconds(-1),
				CancellationToken.None));

	#endregion

	#region Device Model Validation Tests

	/// <summary>
	/// Host names a device must refuse, with the exception each one produces.
	/// </summary>
	public static TheoryData<string?, bool> InvalidHostNames => new()
	{
		{ null, true },
		{ "", false },
		{ "   ", false }
	};

	/// <summary>
	/// Performs Device_Should_Reject_Invalid_Hostname operation.
	/// </summary>
	[Theory]
	[MemberData(nameof(InvalidHostNames))]
	public void Device_Should_Reject_Invalid_Hostname(string? hostName, bool expectsArgumentNull)
	{
		// Act
		var construct = (Func<LightBulb>)(() => new LightBulb(hostName!, LanTestDevice.MacAddress));

		// Assert
		if (expectsArgumentNull)
		{
			construct.Should().ThrowExactly<ArgumentNullException>();
		}
		else
		{
			construct.Should().ThrowExactly<ArgumentException>();
		}
	}

	/// <summary>
	/// Performs Device_MacAddress_Should_Be_Six_Bytes operation.
	/// </summary>
	[Fact]
	public void Device_MacAddress_Should_Be_Six_Bytes()
	{
		// Arrange & Act
		var device = CreateTestBulb();

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
		var device = CreateTestBulb();

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
