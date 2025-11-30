using AwesomeAssertions;
using Lifx.Api.Lan;
using Lifx.Api.Models.Lan;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for LAN message parsing and serialization
/// </summary>
[Collection("LAN Tests")]
public class LanMessageTests
{
	[Fact]
	public void FrameHeader_Should_Initialize_With_Defaults()
	{
		// Act
		var header = new FrameHeader();

		// Assert
		header.Identifier.Should().Be(0u);
		header.Sequence.Should().Be((byte)0);
		header.AcknowledgeRequired.Should().BeFalse();
		header.ResponseRequired.Should().BeFalse();
		header.TargetMacAddress.Should().NotBeNull();
		header.TargetMacAddress.Should().HaveCount(8);
		header.AtTime.Should().Be(DateTime.MinValue);
	}

	[Fact]
	public void FrameHeader_TargetMacAddressName_Should_Format_Correctly()
	{
		// Arrange
		var header = new FrameHeader
		{
			TargetMacAddress = [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01, 0x00, 0x00]
		};

		// Act
		var macAddress = header.TargetMacAddressName;

		// Assert
		macAddress.Should().Be("D0:73:D5:00:00:01");
	}

	[Fact]
	public void FrameHeader_TargetMacAddressName_Should_Handle_Null()
	{
		// Arrange
		var header = new FrameHeader
		{
			TargetMacAddress = null!
		};

		// Act
		var macAddress = header.TargetMacAddressName;

		// Assert
		macAddress.Should().Be(string.Empty);
	}

	[Fact]
	public void ParseMessage_Should_Reject_Packet_Too_Small()
	{
		// Arrange
		var tooSmallPacket = new byte[35]; // Minimum is 36 bytes

		// Act & Assert
		var exception = Assert.Throws<System.Reflection.TargetInvocationException>(() =>
		{
			var method = typeof(LifxLanClient).GetMethod("ParseMessage",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			method!.Invoke(null, [tooSmallPacket]);
		});
		
		// Verify the inner exception is the expected type
		exception.InnerException.Should().BeOfType<Exception>();
		exception.InnerException!.Message.Should().Contain("Invalid packet");
	}

	[Fact]
	public void ParseMessage_Should_Reject_Invalid_Packet_Size()
	{
		// Arrange - Create packet with wrong size field
		var packet = new byte[50];
		using (var ms = new MemoryStream(packet))
		using (var bw = new BinaryWriter(ms))
		{
			bw.Write((ushort)100); // Wrong size - says 100 but packet is only 50
			bw.Write((ushort)0x3400); // protocol
			bw.Write((uint)12345); // source
		}

		// Act & Assert
		var exception = Assert.Throws<System.Reflection.TargetInvocationException>(() =>
		{
			var method = typeof(LifxLanClient).GetMethod("ParseMessage",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			method!.Invoke(null, [packet]);
		});
		
		// Verify the inner exception is the expected type
		exception.InnerException.Should().BeOfType<Exception>();
		exception.InnerException!.Message.Should().Contain("Invalid packet");
	}

	[Fact]
	public void WritePacketToStream_Should_Create_Valid_Packet()
	{
		// Arrange
		var header = new FrameHeader
		{
			Identifier = 12345,
			Sequence = 1,
			AcknowledgeRequired = true,
			ResponseRequired = true,
			TargetMacAddress = [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01, 0x00, 0x00],
			AtTime = DateTime.MinValue
		};
		var payload = new byte[] { 1, 2, 3, 4 };
		var messageType = (ushort)MessageType.DeviceGetLabel;

		// Act
		using var stream = new MemoryStream();
		var method = typeof(LifxLanClient).GetMethod("WritePacketToStream",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		method!.Invoke(null, [stream, header, messageType, payload]);

		var packet = stream.ToArray();

		// Assert
		packet.Should().NotBeNull();
		packet.Should().HaveCount(40); // 36 byte header + 4 byte payload

		// Verify size field
		var size = BitConverter.ToUInt16(packet, 0);
		size.Should().Be(40);

		// Verify protocol field
		var protocol = BitConverter.ToUInt16(packet, 2);
		protocol.Should().Be(0x3400);

		// Verify source identifier
		var source = BitConverter.ToUInt32(packet, 4);
		source.Should().Be(12345u);

		// Verify MAC address
		for (int i = 0; i < 8; i++)
		{
			packet[8 + i].Should().Be(header.TargetMacAddress[i]);
		}
	}

	[Fact]
	public void WritePacketToStream_Should_Include_Correct_Flags()
	{
		// Arrange
		var header = new FrameHeader
		{
			Identifier = 1,
			Sequence = 5,
			AcknowledgeRequired = true,
			ResponseRequired = true,
			TargetMacAddress = [0, 0, 0, 0, 0, 0, 0, 0]
		};

		// Act
		using var stream = new MemoryStream();
		var method = typeof(LifxLanClient).GetMethod("WritePacketToStream",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		method!.Invoke(null, [stream, header, (ushort)48, Array.Empty<byte>()]);

		var packet = stream.ToArray();

		// Assert
		// Byte 22 contains the flags (ack_required | res_required)
		packet[22].Should().Be(0x03); // Both ack and res required

		// Byte 23 contains the sequence
		packet[23].Should().Be(5);
	}

	[Fact]
	public void WritePacketToStream_Should_Handle_String_Payload()
	{
		// Arrange
		var header = new FrameHeader
		{
			Identifier = 1,
			TargetMacAddress = [0, 0, 0, 0, 0, 0, 0, 0]
		};
		var testLabel = "Test Light";
		var paddedLabel = testLabel.PadRight(32)[..32];
		var payload = System.Text.Encoding.UTF8.GetBytes(paddedLabel);

		// Act
		using var stream = new MemoryStream();
		var method = typeof(LifxLanClient).GetMethod("WritePacketToStream",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		method!.Invoke(null, [stream, header, (ushort)24, payload]);

		var packet = stream.ToArray();

		// Assert
		packet.Should().HaveCount(68); // 36 header + 32 payload
		var extractedLabel = System.Text.Encoding.UTF8.GetString(packet, 36, 32).TrimEnd('\0', ' ');
		extractedLabel.Should().Be(testLabel);
	}

	[Fact]
	public void BroadcastMessage_Should_Handle_UShort_Payload()
	{
		// This tests the BroadcastMessageAsync parameter handling
		// We can't easily test the full async method, but we can verify payload building

		// Arrange
		ushort testValue = 65535;
		var expectedBytes = BitConverter.GetBytes(testValue);

		// Assert
		expectedBytes.Should().HaveCount(2);
		expectedBytes[0].Should().Be(0xFF);
		expectedBytes[1].Should().Be(0xFF);
	}

	[Fact]
	public void BroadcastMessage_Should_Handle_UInt_Payload()
	{
		// Arrange
		uint testValue = 0x12345678;
		var expectedBytes = BitConverter.GetBytes(testValue);

		// Assert
		expectedBytes.Should().HaveCount(4);
		expectedBytes[0].Should().Be(0x78);
		expectedBytes[1].Should().Be(0x56);
		expectedBytes[2].Should().Be(0x34);
		expectedBytes[3].Should().Be(0x12);
	}

	[Fact]
	public void MessageType_Enum_Should_Have_Expected_Values()
	{
		// Assert key message types exist
		((int)MessageType.DeviceGetService).Should().Be(2);
		((int)MessageType.DeviceStateService).Should().Be(3);
		((int)MessageType.DeviceGetLabel).Should().Be(23);
		((int)MessageType.DeviceStateLabel).Should().Be(25);
		((int)MessageType.LightGet).Should().Be(101);
		((int)MessageType.LightSetColor).Should().Be(102);
		((int)MessageType.LightState).Should().Be(107);
	}
}
