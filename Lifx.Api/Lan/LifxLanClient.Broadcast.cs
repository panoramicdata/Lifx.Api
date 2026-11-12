
namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

public partial class LifxLanClient
{
	private Task<T?> BroadcastMessageAsync<T>(
		string? hostName,
		FrameHeader header,
		MessageType type,
		CancellationToken cancellationToken,
		params object[] args) where T : LifxResponse
	{
		var payload = new List<byte>();
		if (args is not null)
		{
			foreach (var arg in args)
			{
				switch (arg)
				{
					case ushort:
						payload.AddRange(BitConverter.GetBytes((ushort)arg));
						break;
					case uint:
						payload.AddRange(BitConverter.GetBytes((uint)arg));
						break;
					case byte:
						payload.Add((byte)arg);
						break;
					case byte[]:
						payload.AddRange((byte[])arg);
						break;
					case string:
						payload.AddRange(Encoding.UTF8.GetBytes([.. ((string)arg).PadRight(32).Take(32)])); //All strings are 32 bytes
						break;
					default:
						throw new NotSupportedException(args.GetType().FullName);
				}
			}
		}

		return BroadcastMessagePayloadAsync<T>(
			hostName,
			header,
			type,
			[.. payload],
			cancellationToken);
	}

	private async Task<T?> BroadcastMessagePayloadAsync<T>(
		string? hostName,
		FrameHeader header,
		MessageType type,
		byte[] payload,
		CancellationToken cancellationToken)
		where T : LifxResponse
	{
		if (_socket is null)
		{
			throw new InvalidOperationException("No valid socket");
		}

		hostName ??= "255.255.255.255";

		TaskCompletionSource<T>? tcs = null;
		if (//header.AcknowledgeRequired &&
			header.Identifier > 0 &&
			typeof(T) != typeof(UnknownResponse))
		{
			tcs = new TaskCompletionSource<T>();
			void action(LifxResponse r)
			{
				if (r.GetType() == typeof(T))
				{
					tcs.TrySetResult((T)r);
				}
			}

			taskCompletions[header.Identifier] = action;
		}

		using (var stream = new MemoryStream())
		{
			WritePacketToStream(stream, header, (ushort)type, payload);
			var msg = stream.ToArray();
			await _socket.SendAsync(msg, msg.Length, hostName, Port);
		}

		T? result = default;
		if (tcs is not null)
		{
			_ = Task.Delay(1000, cancellationToken).ContinueWith((t) =>
			{
				if (!t.IsCompleted)
				{
					tcs.TrySetException(new TimeoutException());
				}
			}, cancellationToken);
			try
			{
				result = await tcs.Task.ConfigureAwait(false);
			}
			finally
			{
				taskCompletions.Remove(header.Identifier);
			}
		}

		return result;
	}
}
