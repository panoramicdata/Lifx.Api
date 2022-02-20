using Microsoft.Extensions.Logging;

namespace Lifx.Api.Lan
{
    public partial class LifxLanClient : IDisposable
    {
        /// <summary>
        /// Turns the device on
        /// </summary>
        public Task TurnDeviceOnAsync(Device device) => SetDevicePowerStateAsync(device, true);

        /// <summary>
        /// Turns the device off
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Task TurnDeviceOffAsync(Device device) => SetDevicePowerStateAsync(device, false);

        /// <summary>
        /// Sets the device power state
        /// </summary>
        /// <param name="device"></param>
        /// <param name="isOn"></param>
        /// <returns></returns>
        public async Task SetDevicePowerStateAsync(Device device, bool isOn)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));
            _logger.LogTrace("Sending DeviceSetPower({IsOn}) to {DeviceHostName}", isOn, device.HostName);
            FrameHeader header = new()
            {
                Identifier = GetNextIdentifier(),
                AcknowledgeRequired = true
            };

            _ = await BroadcastMessageAsync<AcknowledgementResponse>(device.HostName, header,
                MessageType.DeviceSetPower, (ushort)(isOn ? 65535 : 0)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the label for the device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public async Task<string?> GetDeviceLabelAsync(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            FrameHeader header = new()
            {
                Identifier = GetNextIdentifier(),
                AcknowledgeRequired = false
            };
            var resp = await BroadcastMessageAsync<StateLabelResponse>(device.HostName, header, MessageType.DeviceGetLabel).ConfigureAwait(false);
            return resp.Label;
        }

        /// <summary>
        /// Sets the label on the device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public async Task SetDeviceLabelAsync(Device device, string label)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            FrameHeader header = new()
            {
                Identifier = GetNextIdentifier(),
                AcknowledgeRequired = true
            };
            _ = await BroadcastMessageAsync<AcknowledgementResponse>(
                device.HostName, header, MessageType.DeviceSetLabel, label).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the device version
        /// </summary>
        public Task<StateVersionResponse> GetDeviceVersionAsync(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            FrameHeader header = new()
            {
                Identifier = GetNextIdentifier(),
                AcknowledgeRequired = false
            };
            return BroadcastMessageAsync<StateVersionResponse>(device.HostName, header, MessageType.DeviceGetVersion);
        }
        /// <summary>
        /// Gets the device's host firmware
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Task<StateHostFirmwareResponse> GetDeviceHostFirmwareAsync(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            FrameHeader header = new()
            {
                Identifier = GetNextIdentifier(),
                AcknowledgeRequired = false
            };
            return BroadcastMessageAsync<StateHostFirmwareResponse>(device.HostName, header, MessageType.DeviceGetHostFirmware);
        }
    }
}
