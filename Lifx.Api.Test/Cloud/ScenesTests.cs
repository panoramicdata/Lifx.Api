using AwesomeAssertions;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

[Collection("Cloud API Tests")]
public class ScenesTests(ITestOutputHelper testOutputHelper) : Test(testOutputHelper)
{
	[Fact]
	public async Task ListScenesAsync_Should_Return_Scenes()
	{
		// Act
		var scenes = await Client.Scenes.ListScenesAsync(CancellationToken);

		// Assert
		scenes.Should().NotBeNull();
		Logger.LogInformation("Found {Count} scenes", scenes.Count);
	}

	[Fact]
	public async Task ActivateScene_Should_Succeed_When_Scene_Exists()
	{
		// Arrange
		var scenes = await Client.Scenes.ListScenesAsync(CancellationToken);

		if (scenes.Count == 0)
		{
			Logger.LogWarning("No scenes found to test activation");
			return; // Skip test if no scenes configured
		}

		var firstScene = scenes[0];
		var request = new ActivateSceneRequest
		{
			Duration = 1.0,
			Fast = false
		};

		// Act
		var response = await Client.Scenes.ActivateSceneAsync(
			$"scene_id:{firstScene.Uuid}",
			request,
			CancellationToken);

		// Assert
		response.Should().NotBeNull();
		response.IsSuccessStatusCode.Should().BeTrue();
		Logger.LogInformation("Activated scene: {Name}", firstScene.Name);
	}

	[Fact]
	public async Task ActivateScene_Fast_Should_Succeed()
	{
		// Arrange
		var scenes = await Client
			.Scenes
			.ListScenesAsync(CancellationToken);

		if (scenes.Count == 0)
		{
			Logger.LogWarning("No scenes found to test fast activation");
			return;
		}

		var firstScene = scenes[0];
		var request = new ActivateSceneRequest
		{
			Duration = 0.5,
			Fast = true
		};

		// Act
		var response = await Client
			.Scenes
			.ActivateSceneAsync(
				$"scene_id:{firstScene.Uuid}",
				request,
				CancellationToken);

		// Assert
		response.Should().NotBeNull();
		response.IsSuccessStatusCode.Should().BeTrue();
	}
}
