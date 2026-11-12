namespace Lifx.Api.Lan;

internal class FrameHeader
{
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public uint Identifier;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public byte Sequence;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public bool AcknowledgeRequired;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public bool ResponseRequired;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public byte[] TargetMacAddress;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public DateTime AtTime;
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public FrameHeader()
	{
		Identifier = 0;
		Sequence = 0;
		AcknowledgeRequired = false;
		ResponseRequired = false;
		TargetMacAddress = [0, 0, 0, 0, 0, 0, 0, 0];
		AtTime = DateTime.MinValue;
	}
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public string TargetMacAddressName
	{
		get
		{
			if (TargetMacAddress is null)
			{
				return string.Empty;
			}

			return string.Join(":", TargetMacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray());
		}
	}
}
