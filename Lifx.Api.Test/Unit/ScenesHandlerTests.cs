using AwesomeAssertions;
using Lifx.Api.Models.Cloud.Responses;
using Lifx.Cli.Handlers;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for ScenesHandler.
/// </summary>
[Collection("Unit Tests")]
public class ScenesHandlerTests
{
	private static readonly Account TestAccount = new() { UUID = "test-account" };

	/// <summary>
	/// Tests FindScene by name returns correct match.
	/// </summary>
	[Fact]
	public void FindScene_ByName_ReturnsMatch()
	{
		var scenes = CreateScenes();

		var result = ScenesHandler.FindScene(scenes, "Movie Time");

		result.Should().NotBeNull();
		result!.Name.Should().Be("Movie Time");
	}

	/// <summary>
	/// Tests FindScene by name is case insensitive.
	/// </summary>
	[Fact]
	public void FindScene_ByName_CaseInsensitive()
	{
		var scenes = CreateScenes();

		var result = ScenesHandler.FindScene(scenes, "movie time");

		result.Should().NotBeNull();
		result!.Name.Should().Be("Movie Time");
	}

	/// <summary>
	/// Tests FindScene by UUID returns correct match.
	/// </summary>
	[Fact]
	public void FindScene_ByUuid_ReturnsMatch()
	{
		var scenes = CreateScenes();

		var result = ScenesHandler.FindScene(scenes, "uuid-001");

		result.Should().NotBeNull();
		result!.Uuid.Should().Be("uuid-001");
	}

	/// <summary>
	/// Tests FindScene returns null when not found.
	/// </summary>
	[Fact]
	public void FindScene_NotFound_ReturnsNull()
	{
		var scenes = CreateScenes();

		var result = ScenesHandler.FindScene(scenes, "nonexistent");

		result.Should().BeNull();
	}

	/// <summary>
	/// Tests BuildSceneSelector formats correctly.
	/// </summary>
	[Fact]
	public void BuildSceneSelector_FormatsCorrectly()
	{
		var result = ScenesHandler.BuildSceneSelector("abc-123");

		result.Should().Be("scene_id:abc-123");
	}

	/// <summary>
	/// Tests BuildActivateRequest sets properties correctly.
	/// </summary>
	[Fact]
	public void BuildActivateRequest_SetsProperties()
	{
		var request = ScenesHandler.BuildActivateRequest(2.5, true);

		request.Duration.Should().Be(2.5);
		request.Fast.Should().BeTrue();
	}

	/// <summary>
	/// Tests BuildActivateRequest with fast false.
	/// </summary>
	[Fact]
	public void BuildActivateRequest_FastFalse()
	{
		var request = ScenesHandler.BuildActivateRequest(1.0, false);

		request.Fast.Should().BeFalse();
	}

	private static List<Scene> CreateScenes() =>
	[
		new Scene { Name = "Movie Time", Uuid = "uuid-001", Account = TestAccount, States = [], CreatedAt = 0, UpdatedAt = 0 },
		new Scene { Name = "Reading", Uuid = "uuid-002", Account = TestAccount, States = [], CreatedAt = 0, UpdatedAt = 0 },
		new Scene { Name = "Party", Uuid = "uuid-003", Account = TestAccount, States = [], CreatedAt = 0, UpdatedAt = 0 }
	];
}
