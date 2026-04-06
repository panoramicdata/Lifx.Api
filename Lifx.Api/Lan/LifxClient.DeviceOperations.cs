using Microsoft.Extensions.Logging;

namespace Lifx.Api.Lan;

using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Lan;

/// <summary>
/// Represents the LifxLanClient type.
/// </summary>
public partial class LifxLanClient : IDisposable
{
	/// <summary>
	/// Performs SetDevicePowerStateAsync operation.
	/// </summary>
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
	/// Performs GetDeviceLabelAsync operation.
	/// </summary>
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
	/// Performs SetDeviceLabelAsync operation.
	/// </summary>
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
	/// Performs GetDeviceVersionAsync operation.
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
	/// Performs GetDeviceHostFirmwareAsync operation.
	/// </summary>
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
