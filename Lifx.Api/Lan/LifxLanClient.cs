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
			_socket = CreateSocket(end);
			_receiveLoopTask = ReceiveLoopAsync(cancellationToken);
			logger.LogTrace("Initialization complete.");
		}
	}

	private static UdpClient CreateSocket(IPEndPoint end)
	{
		var socket = new UdpClient(end);
		try
		{
			socket.Client.Blocking = false;
			socket.DontFragment = true;
			socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			return socket;
		}
		catch
		{
			socket.Dispose();
			throw;
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
		var msg = LifxPacketCodec.ParseMessage(data);
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

	/// <summary>
	/// Performs Dispose operation.
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases the socket and cancellation sources.
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing && MarkDisposed())
		{
			// Discovery is cancelled before the main token so its loop stops broadcasting before the
			// socket goes away.
			IgnoringObjectDisposed(() => _DiscoverCancellationSource?.Cancel());
			IgnoringObjectDisposed(_cancellationTokenSource.Cancel);

			WaitForReceiveLoop();

			if (_socket is not null)
			{
				_socket.Dispose();
			}

			IgnoringObjectDisposed(() => _DiscoverCancellationSource?.Dispose());
			try
			{
				_cancellationTokenSource.Dispose();
			}
			catch (ObjectDisposedException)
			{
				// Already disposed by a racing teardown step; nothing left to release.
			}
		}
	}

	private bool MarkDisposed()
	{
		lock (_disposeLock)
		{
			if (_disposed)
			{
				return false;
			}

			_disposed = true;
			return true;
		}
	}

	private static void IgnoringObjectDisposed(Action action)
	{
		try
		{
			action();
		}
		catch (ObjectDisposedException)
		{
			// Teardown races the receive and discovery loops, so whatever this step touches may
			// already have been disposed. That is the expected outcome here, not a failure.
		}
	}

	private void WaitForReceiveLoop()
	{
		if (_receiveLoopTask is null)
		{
			return;
		}

		try
		{
			_receiveLoopTask.Wait(TimeSpan.FromSeconds(5));
		}
		catch (AggregateException)
		{
			// The loop was cancelled or faulted; either way there is nothing left to wait for.
		}
	}
}
