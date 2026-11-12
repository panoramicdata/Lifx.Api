using Microsoft.Extensions.Logging;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

public partial class LifxLanClient
{
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
				catch (Exception ex)
				{
					// A failed broadcast must not end discovery: the socket may be momentarily
					// unavailable, and the next pass five seconds later usually succeeds.
					logger.LogDebug(ex, "Discovery broadcast failed: {Message}", ex.Message);
				}

				await Task.Delay(5000, cancellationToken);
				var lostDevices = devices.Where(d => (DateTime.UtcNow - d.LastSeen).TotalMinutes > 5).ToArray();
				if (lostDevices.Length != 0)
				{
					foreach (var device in lostDevices)
					{
						devices.Remove(device);
						_discoveredBulbs.Remove(device.MacAddressName);
						OnDeviceLost(device);
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
