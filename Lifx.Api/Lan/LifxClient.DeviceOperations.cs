using Microsoft.Extensions.Logging;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;

public partial class LifxLanClient : IDisposable
{
	/// <summary>
	/// Sets the device power state
	/// </summary>
	/// <param name="device"></param>
	/// <param name="isOn"></param>
	/// <returns></returns>
	public async Task SetDevicePowerStateAsync(
		Device device,
		PowerState powerState,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(device);

		var isOn = powerState == PowerState.On;

		logger.LogTrace(
			"Sending DeviceSetPower({IsOn}) to {DeviceHostName}",
			isOn,
			device.HostName);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};

		_ = await BroadcastMessageAsync<AcknowledgementResponse>(
			device.HostName,
			header,
			MessageType.DeviceSetPower,
			cancellationToken,
			(ushort)(isOn ? 65535 : 0)).ConfigureAwait(false);
	}

	/// <summary>
	/// Gets the label for the device
	/// </summary>
	/// <param name="device"></param>
	/// <returns></returns>
	public async Task<string?> GetDeviceLabelAsync(
		Device device,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(device);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = false
		};
		var resp = await BroadcastMessageAsync<StateLabelResponse>(
			device.HostName,
			header,
			MessageType.DeviceGetLabel,
			cancellationToken)
			.ConfigureAwait(false);
		return resp?.Label;
	}

	/// <summary>
	/// Sets the label on the device
	/// </summary>
	/// <param name="device"></param>
	/// <param name="label"></param>
	/// <returns></returns>
	public async Task SetDeviceLabelAsync(
		Device device,
		string label,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(device);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = true
		};
		_ = await BroadcastMessageAsync<AcknowledgementResponse>(
			device.HostName, header, MessageType.DeviceSetLabel, cancellationToken, label).ConfigureAwait(false);
	}

	/// <summary>
	/// Gets the device version
	/// </summary>
	public async Task<StateVersionResponse?> GetDeviceVersionAsync(
		Device device,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(device);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = false
		};

		return await BroadcastMessageAsync<StateVersionResponse>(
			device.HostName,
			header,
			MessageType.DeviceGetVersion,
			cancellationToken);
	}

	/// <summary>
	/// Gets the device's host firmware
	/// </summary>
	/// <param name="device"></param>
	/// <returns></returns>
	public async Task<StateHostFirmwareResponse?> GetDeviceHostFirmwareAsync(
		Device device,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(device);

		FrameHeader header = new()
		{
			Identifier = GetNextIdentifier(),
			AcknowledgeRequired = false
		};
		return await BroadcastMessageAsync<StateHostFirmwareResponse>(
			device.HostName,
			header,
			MessageType.DeviceGetHostFirmware,
			cancellationToken);
	}
}
