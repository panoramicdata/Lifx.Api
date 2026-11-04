namespace Lifx.Api;

/// <summary>
/// Refuses every request that is not a read while <see cref="LifxClientOptions.IsReadOnly"/> is set.
/// </summary>
internal sealed class ReadOnlyGuardHandler(LifxClientOptions options, HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (options.IsReadOnly
			&& request.Method != HttpMethod.Get
			&& request.Method != HttpMethod.Head
			&& request.Method != HttpMethod.Options)
		{
			throw new InvalidOperationException($"The client is read-only (IsReadOnly is true): {request.Method} requests are not allowed.");
		}

		return base.SendAsync(request, cancellationToken);
	}
}
