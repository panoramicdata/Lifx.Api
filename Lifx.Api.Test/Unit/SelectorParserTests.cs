using AwesomeAssertions;
using Lifx.Api.Models.Cloud;
using Lifx.Cli;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for CLI SelectorParser.
/// </summary>
[Collection("Unit Tests")]
public class SelectorParserTests
{
	/// <summary>
	/// Tests that null or whitespace input returns All selector.
	/// </summary>
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("  ")]
	public void ParseSelector_NullOrWhitespace_Returns_All(string? input)
	{
		var result = SelectorParser.ParseSelector(input!);
		result.Should().Be(Selector.All);
	}

	/// <summary>
	/// Tests that "all" (case-insensitive) returns All selector.
	/// </summary>
	[Theory]
	[InlineData("all")]
	[InlineData("ALL")]
	[InlineData("All")]
	public void ParseSelector_All_CaseInsensitive_Returns_All(string input)
	{
		var result = SelectorParser.ParseSelector(input);
		result.Should().Be(Selector.All);
	}

	/// <summary>
	/// Tests that id: prefix returns LightId selector.
	/// </summary>
	[Fact]
	public void ParseSelector_Id_Prefix_Returns_LightId()
	{
		var result = SelectorParser.ParseSelector("id:d073d5000001");
		result.Should().BeOfType<Selector.LightId>();
		result.ToString().Should().Be("id:d073d5000001");
	}

	/// <summary>
	/// Tests that light_id: prefix returns LightId selector.
	/// </summary>
	[Fact]
	public void ParseSelector_LightId_Prefix_Returns_LightId()
	{
		var result = SelectorParser.ParseSelector("light_id:abc123");
		result.Should().BeOfType<Selector.LightId>();
		result.ToString().Should().Be("id:abc123");
	}

	/// <summary>
	/// Tests that label: prefix returns LightLabel selector.
	/// </summary>
	[Fact]
	public void ParseSelector_Label_Prefix_Returns_LightLabel()
	{
		var result = SelectorParser.ParseSelector("label:Kitchen Light");
		result.Should().BeOfType<Selector.LightLabel>();
		result.ToString().Should().Be("label:Kitchen Light");
	}

	/// <summary>
	/// Tests that group: prefix returns GroupId selector.
	/// </summary>
	[Fact]
	public void ParseSelector_Group_Prefix_Returns_GroupId()
	{
		var result = SelectorParser.ParseSelector("group:abc-def");
		result.Should().BeOfType<Selector.GroupId>();
	}

	/// <summary>
	/// Tests that group_id: prefix returns GroupId selector.
	/// </summary>
	[Fact]
	public void ParseSelector_GroupId_Prefix_Returns_GroupId()
	{
		var result = SelectorParser.ParseSelector("group_id:abc-def");
		result.Should().BeOfType<Selector.GroupId>();
	}

	/// <summary>
	/// Tests that group_label: prefix returns GroupLabel selector.
	/// </summary>
	[Fact]
	public void ParseSelector_GroupLabel_Prefix_Returns_GroupLabel()
	{
		var result = SelectorParser.ParseSelector("group_label:Living Room");
		result.Should().BeOfType<Selector.GroupLabel>();
		result.ToString().Should().Be("group:Living Room");
	}

	/// <summary>
	/// Tests that location: prefix returns LocationId selector.
	/// </summary>
	[Fact]
	public void ParseSelector_Location_Prefix_Returns_LocationId()
	{
		var result = SelectorParser.ParseSelector("location:loc-1");
		result.Should().BeOfType<Selector.LocationId>();
	}

	/// <summary>
	/// Tests that location_id: prefix returns LocationId selector.
	/// </summary>
	[Fact]
	public void ParseSelector_LocationId_Prefix_Returns_LocationId()
	{
		var result = SelectorParser.ParseSelector("location_id:loc-1");
		result.Should().BeOfType<Selector.LocationId>();
	}

	/// <summary>
	/// Tests that location_label: prefix returns LocationLabel selector.
	/// </summary>
	[Fact]
	public void ParseSelector_LocationLabel_Prefix_Returns_LocationLabel()
	{
		var result = SelectorParser.ParseSelector("location_label:Home");
		result.Should().BeOfType<Selector.LocationLabel>();
		result.ToString().Should().Be("location:Home");
	}

	/// <summary>
	/// Tests that unknown prefix throws ArgumentException.
	/// </summary>
	[Fact]
	public void ParseSelector_Unknown_Prefix_Throws()
	{
		var act = () => SelectorParser.ParseSelector("unknown:value");
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Unknown selector type*unknown*");
	}

	/// <summary>
	/// Tests that plain text without colon returns LightLabel.
	/// </summary>
	[Fact]
	public void ParseSelector_PlainText_Returns_LightLabel()
	{
		var result = SelectorParser.ParseSelector("Bedroom");
		result.Should().BeOfType<Selector.LightLabel>();
		result.ToString().Should().Be("label:Bedroom");
	}

	/// <summary>
	/// Tests that value with colons preserves everything after first colon.
	/// </summary>
	[Fact]
	public void ParseSelector_Value_With_Colons_Preserves_Full_Value()
	{
		var result = SelectorParser.ParseSelector("label:Light:With:Colons");
		result.Should().BeOfType<Selector.LightLabel>();
		result.ToString().Should().Be("label:Light:With:Colons");
	}
}
