using Microsoft.Extensions.Logging;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;

/// <summary>
/// Represents the LifxLanClient type.
/// </summary>
public partial class LifxLanClient : IDisposable
{
	private readonly Dictionary<uint, Action<LifxResponse>> taskCompletions = [];

	/// <summary>
	/// Performs SetLightPowerAsync operation.
	/// </summary>
	public async Task SetLightPowerAsync(
		LightBulb bulb,
		TimeSpan transitionDuration,
		PowerState powerState,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		if (transitionDuration.TotalMilliseconds > uint.MaxValue ||
			transitionDuration.Ticks < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(transitionDuration));
		}

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};

		var b = BitConverter.GetBytes((ushort)transitionDuration.TotalMilliseconds);

		var isOn = powerState == PowerState.On;

		logger.LogTrace(
			"Sending LightSetPower(on={IsOn}, duration={TransitionDurationTotalMilliseconds}ms) to {HostName}",
			isOn,
			transitionDuration.TotalMilliseconds,
			bulb.HostName);

		await BroadcastMessageAsync<AcknowledgementResponse>(
			bulb.HostName,
			header,
			MessageType.LightSetPower,
			cancellationToken,
			(ushort)(isOn ? 65535 : 0),
			b
		).ConfigureAwait(false);
	}

	/// <summary>
	/// Performs GetLightPowerAsync operation.
	/// </summary>
	public async Task<bool> GetLightPowerAsync(
		LightBulb bulb,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};

		var lightPowerResponse = await BroadcastMessageAsync<LightPowerResponse>(
			bulb.HostName,
			header,
			MessageType.LightGetPower,
			cancellationToken).ConfigureAwait(false);

		return lightPowerResponse?.IsOn ?? false;
	}

	/// <summary>
	/// Performs SetColorAsync operation.
	/// </summary>
	public Task SetColorAsync(
		LightBulb bulb,
		Color color,
		ushort kelvin,
		CancellationToken cancellationToken)
		=> SetColorAsync(
			bulb,
			color,
			kelvin,
			TimeSpan.Zero,
			cancellationToken);

	/// <summary>
	/// Performs SetColorAsync operation.
	/// </summary>
	public Task SetColorAsync(
		LightBulb bulb,
		Color color,
		ushort kelvin,
		TimeSpan transitionDuration,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		var hsl = Utilities.RgbToHsl(color);
		return SetColorAsync(
			bulb,
			hsl[0],
			hsl[1],
			hsl[2],
			kelvin,
			transitionDuration,
			cancellationToken);
	}

	/// <summary>
	/// Performs SetColorAsync operation.
	/// </summary>
	public async Task SetColorAsync(LightBulb bulb,
		ushort hue,
		ushort saturation,
		ushort brightness,
		ushort kelvin,
		TimeSpan transitionDuration,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		if (transitionDuration.TotalMilliseconds > uint.MaxValue ||
			transitionDuration.Ticks < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(transitionDuration));
		}

		if (kelvin < 2500 || kelvin > 9000)
		{
			throw new ArgumentOutOfRangeException(nameof(kelvin), "Kelvin must be between 2500 and 9000");
		}

		logger.LogDebug("Setting color for {HostName}", bulb.HostName);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};

		uint duration = (uint)transitionDuration.TotalMilliseconds;
		await BroadcastMessageAsync<AcknowledgementResponse>(
			bulb.HostName,
			header,
			MessageType.LightSetColor,
			cancellationToken,
			(byte)0x00, //reserved
			hue,
			saturation,
			brightness,
			kelvin, //HSBK
			duration
		);
	}

	/// <summary>
	/// Performs GetLightStateAsync operation.
	/// </summary>
	public async Task<LightStateResponse?> GetLightStateAsync(
		LightBulb bulb,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = false
		};
		return await BroadcastMessageAsync<LightStateResponse>(
			bulb.HostName,
			header,
			MessageType.LightGet,
			cancellationToken);
	}

	public async Task<LightGroupResponse?> GetGroupAsync(
		LightBulb bulb,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = false
		};
		return await BroadcastMessageAsync<LightGroupResponse>(
			bulb.HostName,
			header,
			MessageType.DeviceGetGroup,
			cancellationToken);
	}

	/// <summary>
	/// Performs GetInfraredAsync operation.
	/// </summary>
	public async Task<ushort> GetInfraredAsync(
		LightBulb bulb,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(bulb);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};
		var response = await BroadcastMessageAsync<InfraredStateRespone>(
			bulb.HostName,
			header,
			MessageType.InfraredGet,
			cancellationToken).ConfigureAwait(false);
		return response?.Brightness ?? 0;
	}

	/// <summary>
	/// Performs SetInfraredAsync operation.
	/// </summary>
	public async Task SetInfraredAsync(
		Device device,
		ushort brightness,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(device);

		logger.LogDebug("Sending SetInfrared({Brightness}) to {HostName}", brightness, device.HostName);
		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};

		_ = await BroadcastMessageAsync<AcknowledgementResponse>(
			device.HostName,
			header,
			MessageType.InfraredSet,
			cancellationToken,
			brightness).ConfigureAwait(false);
	}
}
