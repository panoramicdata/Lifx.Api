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
	public event EventHandler<DeviceDiscoveryEventArgs>? DeviceDiscovered;

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public event EventHandler<DeviceDiscoveryEventArgs>? DeviceLost;

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
		DeviceDiscovered?.Invoke(this, new DeviceDiscoveryEventArgs(device));
	}

	/// <summary>
	/// Performs StartDeviceDiscovery operation.
	/// </summary>
	public void StartDeviceDiscovery(CancellationToken cancellationToken)
	{
		if (_DiscoverCancellationSource is not null && !_DiscoverCancellationSource.IsCancellationRequested)
		{
			return;
		}

		_DiscoverCancellationSource = new CancellationTokenSource();
		var token = _DiscoverCancellationSource.Token;
		var source = discoverSourceID = GetNextIdentifier();

		//Start discovery thread
		Task.Run(async () =>
		{
			logger.LogTrace("{Message}", "Sending GetServices");

			FrameHeader header = new()
			{
				Identifier = source
			};

			while (!token.IsCancellationRequested)
			{
				try
				{
					await BroadcastMessageAsync<UnknownResponse>(
						null,
						header,
						MessageType.DeviceGetService,
						cancellationToken);
				}
				catch { }

				await Task.Delay(5000, cancellationToken);
				var lostDevices = devices.Where(d => (DateTime.UtcNow - d.LastSeen).TotalMinutes > 5).ToArray();
				if (lostDevices.Length != 0)
				{
					foreach (var device in lostDevices)
					{
						devices.Remove(device);
						_discoveredBulbs.Remove(device.MacAddressName);
						DeviceLost?.Invoke(this, new DeviceDiscoveryEventArgs(device));
					}
				}
			}
		}, cancellationToken);
	}

	/// <summary>
	/// Performs StopDeviceDiscovery operation.
	/// </summary>
	public void StopDeviceDiscovery()
	{
		if (_DiscoverCancellationSource is null || _DiscoverCancellationSource.IsCancellationRequested)
		{
			return;
		}

		_DiscoverCancellationSource.Cancel();
		_DiscoverCancellationSource = null;
	}
}

