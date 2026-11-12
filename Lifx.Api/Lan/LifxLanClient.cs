using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

/// <summary>
/// Represents the LifxLanClient type.
/// </summary>
public partial class LifxLanClient : IDisposable
{
	private readonly ILogger logger;
	private static readonly DateTime Epoch = new(1970, 01, 01);
	private const int Port = 56700;
	private UdpClient? _socket;
	private Task? _receiveLoopTask;
	private readonly CancellationTokenSource _cancellationTokenSource = new();
	private bool _disposed;
	private readonly Lock _disposeLock = new();
	private readonly Lock _startLock = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="LifxLanClient"/> class.
	/// </summary>
	public LifxLanClient(ILogger logger) => this.logger = logger;

	/// <summary>
	/// Performs Start operation.
	/// </summary>
	public void Start(CancellationToken cancellationToken)
	{
		lock (_startLock)
		{
			if (_receiveLoopTask is not null)
			{
				throw new InvalidOperationException("Already started.");
			}

			logger.LogTrace("Initializing...");
			var end = new IPEndPoint(IPAddress.Any, Port);
			_socket = new UdpClient(end);
			_socket.Client.Blocking = false;
			_socket.DontFragment = true;
			_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_receiveLoopTask = ReceiveLoopAsync(cancellationToken);
			logger.LogTrace("Initialization complete.");
		}
	}

	private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested && _socket is not null)
		{
			try
			{
				var result = await _socket.ReceiveAsync(cancellationToken);
				if (result.Buffer.Length > 0)
				{
					HandleIncomingMessages(result.Buffer, result.RemoteEndPoint);
				}

				await Task.Delay(1, cancellationToken);
			}
			catch (TaskCanceledException ex)
			{
				logger.LogDebug(ex, "Task cancelled");
			}
			catch (Exception ex)
			{
				logger.LogDebug(ex, "Error in receive loop: {Message}", ex.Message);
			}
		}
	}

	private void HandleIncomingMessages(byte[] data, IPEndPoint endpoint)
	{
		var remote = endpoint;
		var msg = ParseMessage(data);
		if (msg.Type == MessageType.DeviceStateService)
		{
			ProcessDeviceDiscoveryMessage(remote.Address, msg);
		}
		else
		{
			if (taskCompletions.TryGetValue(msg.Source, out Action<LifxResponse>? tcs))
			{
				tcs(msg);
			}
			else
			{
				//TODO
			}
		}

		logger.LogTrace(
			"Received from {Remote}:{Bytes}",
			remote.ToString(),
			string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
	}
}
