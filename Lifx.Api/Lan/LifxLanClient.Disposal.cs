namespace Lifx.Api.Lan;


public partial class LifxLanClient
{
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
		if (!disposing)
		{
			return;
		}

		lock (_disposeLock)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
		}

		// Discovery is cancelled before the main token so its loop stops broadcasting before the
		// socket goes away.
		IgnoringObjectDisposed(() => _DiscoverCancellationSource?.Cancel());
		IgnoringObjectDisposed(_cancellationTokenSource.Cancel);

		WaitForReceiveLoop();

		_socket?.Dispose();

		IgnoringObjectDisposed(() => _DiscoverCancellationSource?.Dispose());
		IgnoringObjectDisposed(_cancellationTokenSource.Dispose);
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
