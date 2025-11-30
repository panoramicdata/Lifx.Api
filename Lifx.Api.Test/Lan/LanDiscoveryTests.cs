using AwesomeAssertions;
using Lifx.Api.Models.Lan;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN device discovery functionality
/// Note: These tests use mock/simulated devices to avoid requiring actual hardware
/// </summary>
[Collection("LAN Tests")]
public class LanDiscoveryTests : IDisposable
{
	private readonly ILogger _logger;
	private readonly LanTestFixture _fixture;
	private LifxClient? _client;

	public LanDiscoveryTests(LanTestFixture fixture)
	{
		_fixture = fixture;
		_logger = LoggerFactory.Create(builder => { })
			.CreateLogger<LanDiscoveryTests>();
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
	public void Shared_LAN_Client_Should_Be_Started()
	{
		// Assert - Use the shared client from fixture
		_fixture.SharedClient.Should().NotBeNull();
		_fixture.SharedClient!.Lan.Should().NotBeNull();
		_fixture.IsLanStarted.Should().BeTrue();
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
		bulb.MacAddress.Length.Should().Be(6);
		bulb.Service.Should().Be(1);
		bulb.Port.Should().Be(56700u);
	}

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
