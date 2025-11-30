namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

/// <summary>
/// Response to any message sent with ack_required set to 1. 
/// </summary>
internal class AcknowledgementResponse : LifxResponse
{
	internal AcknowledgementResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source) { }
}
