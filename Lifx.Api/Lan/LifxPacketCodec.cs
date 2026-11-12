using Lifx.Api.Models.Lan;

namespace Lifx.Api.Lan;

internal static class LifxPacketCodec
{
	private static readonly DateTime Epoch = new(1970, 01, 01);

	internal static LifxResponse ParseMessage(byte[] packet)
	{
		using var memoryStream = new MemoryStream(packet);

		var header = new FrameHeader();
		var binaryReader = new BinaryReader(memoryStream);

		//frame
		var size = binaryReader.ReadUInt16();
		if (packet.Length != size || size < 36)
		{
			throw new Exception("Invalid packet");
		}

		binaryReader.ReadUInt16(); //skip origin:2, reserved:1, addressable:1, protocol:12
		var source = binaryReader.ReadUInt32();

		//frame address
		byte[] target = binaryReader.ReadBytes(8);
		header.TargetMacAddress = target;
		memoryStream.Seek(6, SeekOrigin.Current); //skip reserved
		binaryReader.ReadByte(); //skip reserved:6, ack_required:1, res_required:1
		header.Sequence = binaryReader.ReadByte();

		//protocol header
		var nanoseconds = binaryReader.ReadUInt64();
		header.AtTime = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
		var type = (MessageType)binaryReader.ReadUInt16();
		memoryStream.Seek(2, SeekOrigin.Current); //skip reserved
		return LifxResponse.Create(header, type, source, size > 36 ? binaryReader.ReadBytes(size - 36) : []);
	}

	internal static void WritePacketToStream(Stream outStream, FrameHeader header, ushort type, byte[] payload)
	{
		using var dw = new BinaryWriter(outStream);
		#region Frame
		//size uint16
		dw.Write((ushort)((payload is not null ? payload.Length : 0) + 36)); //length
																			 // origin (2 bits, must be 0), reserved (1 bit, must be 0), addressable (1 bit, must be 1), protocol 12 bits must be 0x400) = 0x1400
		dw.Write((ushort)0x3400); //protocol
		dw.Write(header.Identifier); //source identifier - unique value set by the client, used by responses. If 0, responses are broadcasted instead
		#endregion Frame

		#region Frame address
		//The target device address is 8 bytes long, when using the 6 byte MAC address then left -
		//justify the value and zero-fill the last two bytes. A target device address of all zeroes effectively addresses all devices on the local network
		dw.Write(header.TargetMacAddress); // target mac address - 0 means all devices
		dw.Write([0, 0, 0, 0, 0, 0]); //reserved 1

		//The client can use acknowledgements to determine that the LIFX device has received a message.
		//However, when using acknowledgements to ensure reliability in an over-burdened lossy network ...
		//causing additional network packets may make the problem worse.
		//Client that don't need to track the updated state of a LIFX device can choose not to request a
		//response, which will reduce the network burden and may provide some performance advantage. In
		//some cases, a device may choose to send a state update response independent of whether res_required is set.
		if (header.AcknowledgeRequired && header.ResponseRequired)
		{
			dw.Write((byte)0x03);
		}
		else if (header.AcknowledgeRequired)
		{
			dw.Write((byte)0x02);
		}
		else if (header.ResponseRequired)
		{
			dw.Write((byte)0x01);
		}
		else
		{
			dw.Write((byte)0x00);
		}
		//The sequence number allows the client to provide a unique value, which will be included by the LIFX
		//device in any message that is sent in response to a message sent by the client. This allows the client
		//to distinguish between different messages sent with the same source identifier in the Frame. See
		//ack_required and res_required fields in the Frame Address.
		dw.Write(header.Sequence);
		#endregion Frame address

		#region Protocol Header
		//The at_time value should be zero for Set and Get messages sent by a client.
		//For State messages sent by a device, the at_time will either be the device
		//current time when the message was received or zero. StateColor is an example
		//of a message that will return a non-zero at_time value
		if (header.AtTime > DateTime.MinValue)
		{
			var time = header.AtTime.ToUniversalTime();
			dw.Write((ulong)(time - Epoch).TotalMilliseconds * 10); //timestamp
		}
		else
		{
			dw.Write((ulong)0);
		}
		#endregion Protocol Header
		dw.Write(type); //packet _type
		dw.Write((ushort)0); //reserved
		if (payload is not null)
		{
			dw.Write(payload);
		}

		dw.Flush();
	}
}
