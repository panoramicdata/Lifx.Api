using AwesomeAssertions;
using Lifx.Api.Models.Lan;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN device discovery functionality
/// Note: These tests use mock/simulated devices to avoid requiring actual hardware
/// </summary>
/// <summary>
/// Represents the LanDiscoveryTests type.
/// </summary>
[Collection("LAN Tests")]
public class LanDiscoveryTests(LanTestFixture fixture) : IDisposable
{
	private readonly ILogger _logger = LoggerFactory.Create(builder => { })
			.CreateLogger<LanDiscoveryTests>();
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
	/// Performs LAN_Client_Should_Initialize_When_Enabled operation.
	/// </summary>
	[Fact]
	public void LAN_Client_Should_Initialize_When_Enabled()
	{
		// Arrange & Act
		_client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = true
		});

		// Assert
		_client.Lan.Should().NotBeNull();
	}

	/// <summary>
	/// Performs LAN_Client_Should_Be_Null_When_Disabled operation.
	/// </summary>
	[Fact]
	public void LAN_Client_Should_Be_Null_When_Disabled()
	{
		// Arrange & Act
		_client = new LifxClient(new LifxClientOptions
		{
			Logger = _logger,
			IsLanEnabled = false
		});

		// Assert
		_client.Lan.Should().BeNull();
	}

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
		Assert.Throws<InvalidOperationException>(() =>
			_client.StartLan(CancellationToken.None));
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
		Assert.Throws<InvalidOperationException>(() =>
			_client.StartDeviceDiscovery(CancellationToken.None));
	}

	/// <summary>
	/// Performs Shared_LAN_Client_Should_Be_Started operation.
	/// </summary>
	[Fact]
	public void Shared_LAN_Client_Should_Be_Started()
	{
		if (!fixture.IsLanStarted)
		{
			return;
		}

		// Assert - Use the shared client from fixture
		fixture.SharedClient.Should().NotBeNull();
		fixture.SharedClient!.Lan.Should().NotBeNull();
		fixture.IsLanStarted.Should().BeTrue();
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

	/// <summary>
	/// Performs LightBulb_Should_Initialize_With_Required_Parameters operation.
	/// </summary>
	[Fact]
	public void LightBulb_Should_Initialize_With_Required_Parameters()
	{
		// Arrange & Act
		var bulb = new LightBulb(
			"192.168.1.100",
			[0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01],
			service: 1,
			port: 56700);

		// Assert
		bulb.Should().NotBeNull();
		bulb.HostName.Should().Be("192.168.1.100");
		bulb.MacAddress.Should().NotBeNull();
		bulb.MacAddress.Should().HaveCount(6);
		bulb.Service.Should().Be(1);
		bulb.Port.Should().Be(56700u);
	}

	/// <summary>
	/// Performs LightBulb_Should_Inherit_From_Device operation.
	/// </summary>
	[Fact]
	public void LightBulb_Should_Inherit_From_Device()
	{
		// Arrange & Act
		var bulb = new LightBulb(
			"192.168.1.100",
			[0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Assert
		bulb.Should().BeOfType<LightBulb>();
		(bulb is Device).Should().BeTrue();
		bulb.MacAddress.Should().NotBeNull();
	}

	/// <summary>
	/// Performs LightBulb_MacAddressName_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void LightBulb_MacAddressName_Should_Format_Correctly()
	{
		// Arrange & Act
		var bulb = new LightBulb(
			"192.168.1.100",
			[0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01]);

		// Assert
		bulb.MacAddressName.Should().Be("D0:73:D5:00:00:01");
	}
}
