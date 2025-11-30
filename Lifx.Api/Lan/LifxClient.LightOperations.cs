using Microsoft.Extensions.Logging;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;

public partial class LifxLanClient : IDisposable
{
	private readonly Dictionary<uint, Action<LifxResponse>> taskCompletions = [];

	/// <summary>
	/// Turns a bulb on or off using the provided transition time
	/// </summary>
	/// <param name="bulb"></param>
	/// <param name="transitionDuration"></param>
	/// <param name="isOn">True to turn on, false to turn off</param>
	/// <returns></returns>
	/// <seealso cref="TurnBulbOffAsync(LightBulb, TimeSpan)"/>
	/// <seealso cref="TurnBulbOnAsync(LightBulb, TimeSpan)"/>
	/// <seealso cref="TurnDeviceOnAsync(Device)"/>
	/// <seealso cref="TurnDeviceOffAsync(Device)"/>
	/// <seealso cref="SetDevicePowerStateAsync(Device, bool)"/>
	/// <seealso cref="GetLightPowerAsync(LightBulb)"/>
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
	/// Gets the current power state for a light bulb
	/// </summary>
	/// <param name="bulb"></param>
	/// <returns></returns>
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
	/// Sets color and temperature for a bulb
	/// </summary>
	/// <param name="bulb"></param>
	/// <param name="color"></param>
	/// <param name="kelvin"></param>
	/// <returns></returns>
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
	/// Sets color and temperature for a bulb and uses a transition time to the provided state
	/// </summary>
	/// <param name="bulb"></param>
	/// <param name="color"></param>
	/// <param name="kelvin"></param>
	/// <param name="transitionDuration"></param>
	/// <returns></returns>
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
	/// Sets color and temperature for a bulb and uses a transition time to the provided state
	/// </summary>
	/// <param name="bulb">Light bulb</param>
	/// <param name="hue">0..65535</param>
	/// <param name="saturation">0..65535</param>
	/// <param name="brightness">0..65535</param>
	/// <param name="kelvin">2700..9000</param>
	/// <param name="transitionDuration"></param>
	/// <returns></returns>
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
			duration,
			cancellationToken
		);
	}

	/// <summary>
	/// Gets the current state of the bulb
	/// </summary>
	/// <param name="bulb"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Gets the current maximum power level of the Infrared channel
	/// </summary>
	/// <param name="bulb"></param>
	/// <returns></returns>
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
	/// Sets the infrared brightness level
	/// </summary>
	/// <param name="device"></param>
	/// <param name="brightness"></param>
	/// <returns></returns>
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