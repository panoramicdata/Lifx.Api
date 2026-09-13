using AwesomeAssertions;
using Lifx.Api.Lan;
using Lifx.Api.Models.Lan;
using System.Text;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Tests for parsing the LAN StateGroup message.
/// </summary>
[Collection("Unit Tests")]
public class StateGroupResponseTests
{
	private static readonly Guid GroupId = new("2f1c9d3e-4b5a-6c7d-8e9f-a0b1c2d3e4f5");

	/// <summary>
	/// One day after the epoch, in the nanoseconds the protocol reports. Kept small enough to be
	/// represented exactly as a double, so the conversion cannot drift.
	/// </summary>
	private const ulong OneDayInNanoseconds = 86_400_000_000_000;

	/// <summary>
	/// Builds a StateGroup payload: group GUID (16), label padded with nulls (32), updated_at (8).
	/// </summary>
	private static byte[] BuildPayload(Guid group, string label, ulong updatedAtNanoseconds)
	{
		var payload = new byte[56];

		group.ToByteArray().CopyTo(payload, 0);
		Encoding.UTF8.GetBytes(label).CopyTo(payload, 16);
		BitConverter.GetBytes(updatedAtNanoseconds).CopyTo(payload, 48);

		return payload;
	}

	private static StateGroupResponse Parse(byte[] payload)
		=> (StateGroupResponse)LifxResponse.Create(
			new FrameHeader(),
			MessageType.DeviceStateGroup,
			source: 1,
			payload);

	/// <summary>
	/// Performs Create_Should_Route_StateGroup_To_StateGroupResponse operation.
	/// </summary>
	[Fact]
	public void Create_Should_Route_StateGroup_To_StateGroupResponse()
	{
		// Arrange
		var payload = BuildPayload(GroupId, "Living Room", OneDayInNanoseconds);

		// Act
		var response = LifxResponse.Create(new FrameHeader(), MessageType.DeviceStateGroup, source: 1, payload);

		// Assert
		response.Should().BeOfType<StateGroupResponse>();
	}

	/// <summary>
	/// Performs StateGroup_Should_Parse_Group_Identifier operation.
	/// </summary>
	[Fact]
	public void StateGroup_Should_Parse_Group_Identifier()
	{
		// Act
		var response = Parse(BuildPayload(GroupId, "Living Room", OneDayInNanoseconds));

		// Assert
		response.Group.Should().Be(GroupId);
	}

	/// <summary>
	/// Performs StateGroup_Should_Trim_Null_Padding_From_Label operation.
	/// </summary>
	[Fact]
	public void StateGroup_Should_Trim_Null_Padding_From_Label()
	{
		// Act - the label occupies 32 bytes and is null padded on the wire
		var response = Parse(BuildPayload(GroupId, "Living Room", OneDayInNanoseconds));

		// Assert
		response.Label.Should().Be("Living Room");
	}

	/// <summary>
	/// Performs StateGroup_Should_Handle_Label_Filling_The_Field operation.
	/// </summary>
	[Fact]
	public void StateGroup_Should_Handle_Label_Filling_The_Field()
	{
		// Arrange - exactly 32 characters, so there is no padding to trim
		var label = new string('a', 32);

		// Act
		var response = Parse(BuildPayload(GroupId, label, OneDayInNanoseconds));

		// Assert
		response.Label.Should().Be(label);
	}

	/// <summary>
	/// Performs StateGroup_Should_Convert_UpdatedAt_From_Nanoseconds operation.
	/// </summary>
	[Fact]
	public void StateGroup_Should_Convert_UpdatedAt_From_Nanoseconds()
	{
		// Act
		var response = Parse(BuildPayload(GroupId, "Living Room", OneDayInNanoseconds));

		// Assert
		response.UpdatedAt.Should().Be(new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc));
	}

	/// <summary>
	/// A truncated payload must be reported against the message, not left to fault the receive
	/// loop with an index error from deep inside the parser.
	/// </summary>
	[Theory]
	[InlineData(0)]
	[InlineData(16)]
	[InlineData(48)]
	[InlineData(55)]
	public void StateGroup_Should_Reject_Short_Payload(int length)
	{
		// Arrange
		var payload = new byte[length];

		// Act & Assert
		((Func<LifxResponse>)(() => Parse(payload)))
			.Should()
			.ThrowExactly<ArgumentException>();
	}
}
