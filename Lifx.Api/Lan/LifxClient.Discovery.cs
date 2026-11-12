using Microsoft.Extensions.Logging;
using System.Net;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

/// <summary>
/// Represents the LifxLanClient type.
/// </summary>
public partial class LifxLanClient : IDisposable
{
	private static uint identifier = 2;
	private static readonly Lock identifierLock = new();
	private uint discoverSourceID;
	private CancellationTokenSource? _DiscoverCancellationSource;
	private readonly Dictionary<string, Device> _discoveredBulbs = [];

	private static uint GetNextIdentifier()
	{
		lock (identifierLock)
		{
			return identifier++;
		}
	}

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public event EventHandler<DeviceDiscoveryEventArgs>? DeviceDiscovered
	{
		add { lock (_eventLock) { _deviceDiscovered += value; } }
		remove { lock (_eventLock) { _deviceDiscovered -= value; } }
	}

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public event EventHandler<DeviceDiscoveryEventArgs>? DeviceLost
	{
		add { lock (_eventLock) { _deviceLost += value; } }
		remove { lock (_eventLock) { _deviceLost -= value; } }
	}

	private readonly Lock _eventLock = new();
	private EventHandler<DeviceDiscoveryEventArgs>? _deviceDiscovered;
	private EventHandler<DeviceDiscoveryEventArgs>? _deviceLost;

	private readonly IList<Device> devices = [];

	/// <summary>
	/// Gets or sets Devices.
	/// </summary>
	public IEnumerable<Device> Devices { get { return devices; } }

	/// <summary>
	/// Represents the DeviceDiscoveryEventArgs type.
	/// </summary>
	public sealed class DeviceDiscoveryEventArgs : EventArgs
	{
		internal DeviceDiscoveryEventArgs(Device device) => Device = device;
		/// <summary>
		/// Gets or sets Device.
		/// </summary>
		public Device Device { get; }
	}

	private void ProcessDeviceDiscoveryMessage(IPAddress remoteAddress, LifxResponse msg)
	{
		string id = msg.Header.TargetMacAddressName; //remoteAddress.ToString()
		if (_discoveredBulbs.TryGetValue(id, out Device? value))  //already discovered
		{
			value.LastSeen = DateTime.UtcNow; //Update datestamp
			value.HostName = remoteAddress.ToString(); //Update hostname in case IP changed

			return;
		}

		if (msg.Source != discoverSourceID || //did we request the discovery?
			_DiscoverCancellationSource is null ||
			_DiscoverCancellationSource.IsCancellationRequested) //did we cancel discovery?
		{
			return;
		}

		var device = new LightBulb(remoteAddress.ToString(), msg.Header.TargetMacAddress, msg.Payload[0]
			, BitConverter.ToUInt32(msg.Payload, 1))
		{
			LastSeen = DateTime.UtcNow
		};
		_discoveredBulbs[id] = device;
		devices.Add(device);
		OnDeviceDiscovered(device);
	}

	private void OnDeviceDiscovered(Device device)
		=> _deviceDiscovered?.Invoke(this, new DeviceDiscoveryEventArgs(device));

	private void OnDeviceLost(Device device)
		=> _deviceLost?.Invoke(this, new DeviceDiscoveryEventArgs(device));
}

