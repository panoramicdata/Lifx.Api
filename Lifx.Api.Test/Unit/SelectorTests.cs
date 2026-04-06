using AwesomeAssertions;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Represents the SelectorTests type.
/// </summary>
[Collection("Unit Tests")]
public class SelectorTests
{
	/// <summary>
	/// Performs All_Selector_Should_Return_Correct_String operation.
	/// </summary>
	[Fact]
	public void All_Selector_Should_Return_Correct_String()
	{
		// Arrange & Act
		var selector = Selector.All;

		// Assert
		selector.ToString().Should().Be("all");
	}

	/// <summary>
	/// Performs LightId_Selector_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void LightId_Selector_Should_Format_Correctly()
	{
		// Arrange
		var id = "d073d5000001";

		// Act
		var selector = new Selector.LightId(id);

		// Assert
		selector.ToString().Should().Be("id:d073d5000001");
	}

	/// <summary>
	/// Performs GroupLabel_Selector_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void GroupLabel_Selector_Should_Format_Correctly()
	{
		// Arrange
		var label = "Living Room";

		// Act
		var selector = new Selector.GroupLabel(label);

		// Assert
		selector.ToString().Should().Be("group:Living Room");
	}

	/// <summary>
	/// Performs LocationLabel_Selector_Should_Format_Correctly operation.
	/// </summary>
	[Fact]
	public void LocationLabel_Selector_Should_Format_Correctly()
	{
		// Arrange
		var label = "Home";

		// Act
		var selector = new Selector.LocationLabel(label);

		// Assert
		selector.ToString().Should().Be("location:Home");
	}

	/// <summary>
	/// Performs Explicit_Cast_Should_Parse_Selector_String operation.
	/// </summary>
	[Theory]
	[InlineData("all", "all")]
	[InlineData("id:d073d5000001", "id:d073d5000001")]
	[InlineData("group:Living Room", "group:Living Room")]
	[InlineData("location:Home", "location:Home")]
	public void Explicit_Cast_Should_Parse_Selector_String(string input, string expected)
	{
		// Act
		var selector = (Selector)input;

		// Assert
		selector.ToString().Should().Be(expected);
	}
}
