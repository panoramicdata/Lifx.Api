namespace Lifx.Api.Lan;

internal abstract record LifxPacket
{
	internal ushort Type { get; }
	internal byte[] Payload { get; }

	protected LifxPacket(ushort type, byte[] payload)
	{
		Type = type;
		Payload = payload;
	}

	protected LifxPacket(ushort type, object[] data)
	{
		Type = type;
		using var ms = new MemoryStream();
		var streamWriter = new StreamWriter(ms);
		foreach (var obj in data)
		{
			switch (obj)
			{
				case byte:
					streamWriter.Write((byte)obj);
					break;
				case byte[]:
					streamWriter.Write((byte[])obj);
					break;
				case ushort:
					streamWriter.Write((ushort)obj);
					break;
				case uint:
					streamWriter.Write((uint)obj);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		Payload = ms.ToArray();
	}

	public static LifxPacket FromByteArray(byte[] data)
	{
		//			preambleFields = [
		//				{ name: "size"       , type:type.uint16_le },
		//				{ name: "protocol"   , type:type.uint16_le },
		//				{ name: "reserved1"  , type:type.byte4 }    ,
		//				{ name: "bulbAddress", type:type.byte6 }    ,
		//				{ name: "reserved2"  , type:type.byte2 }    ,
		//				{ name: "site"       , type:type.byte6 }    ,
		//				{ name: "reserved3"  , type:type.byte2 }    ,
		//				{ name: "timestamp"  , type:type.uint64 }   ,
		//				{ name: "packetType" , type:type.uint16_le },
		//				{ name: "reserved4"  , type:type.byte2 }    ,
		//			];
		MemoryStream ms = new(data);
		var br = new BinaryReader(ms);
		//Header
		ushort len = br.ReadUInt16(); //ReverseBytes(br.ReadUInt16()); //size uint16
		br.ReadUInt16(); // Protocol: ReverseBytes(br.ReadUInt16()); //origin = 0
		br.ReadUInt32(); // Identifier
		byte[] bulbAddress = br.ReadBytes(6);
		br.ReadBytes(2); // Reserved 2
		byte[] site = br.ReadBytes(6);
		br.ReadBytes(2); // Reserved 3
		ulong timestamp = br.ReadUInt64();
		ushort packetType = br.ReadUInt16(); // ReverseBytes(br.ReadUInt16());
		br.ReadBytes(2); // Reserved 4
		byte[] payload = [];
		if (len > 0)
		{
			payload = br.ReadBytes(len);
		}

		LifxPacket packet = new UnknownPacket(packetType, payload, bulbAddress, site)
		{
			TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp),
		};

		//packet.Identifier = identifier;

		return packet;
	}

	private record UnknownPacket : LifxPacket
	{
		public UnknownPacket(ushort packetType, byte[] payload, byte[] bulbAddress, byte[] site) : base(packetType, payload)
		{
			BulbAddress = bulbAddress;
			Site = site;
		}

		public byte[] BulbAddress { get; }

		public DateTime TimeStamp { get; set; }

		public byte[] Site { get; set; }
	}
}
