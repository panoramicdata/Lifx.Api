using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;
using Microsoft.Extensions.Logging;

namespace Lifx.Api.Test.Cloud;

/// <summary>
/// Base class for cloud tests that change the state of real lights.
/// </summary>
/// <remarks>
/// Every light's state is recorded before the class runs and put back afterwards, so a test that
/// fails part way through cannot leave the room lit the wrong way. Each test class used to carry
/// its own copy of this; the copies had drifted apart in their logging but not in what they did.
/// </remarks>
public abstract class CloudLightStateTest(ITestOutputHelper testOutputHelper)
	: Test(testOutputHelper), IAsyncLifetime
{
	/// <summary>
	/// Gets the state every light was in before this class ran, or null if it could not be read.
	/// </summary>
	protected List<Light>? OriginalLightStates { get; private set; }

	/// <summary>
	/// Gets the light used by single-light tests.
	/// </summary>
	protected Light? TestLight { get; private set; }

	/// <summary>
	/// Resolves any further fixtures a derived class needs, once the light states are captured.
	/// </summary>
	protected virtual Task OnInitializedAsync() => Task.CompletedTask;

	/// <summary>
	/// Runs before the light states are restored, for anything that must be stopped first.
	/// </summary>
	protected virtual Task OnDisposingAsync() => Task.CompletedTask;

	async ValueTask IAsyncLifetime.InitializeAsync()
	{
		// Capture the original state of all lights before tests run
		try
		{
			OriginalLightStates = await Client.Lights.ListAsync(Selector.All, CancellationToken);
			Logger.LogInformation("Captured original state of {Count} lights", OriginalLightStates.Count);
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Could not capture original light states");
			OriginalLightStates = null;
			return;
		}

		// Resolved separately: a missing test light or group must not discard the states just
		// captured, because those are what puts the lights back the way they were.
		try
		{
			TestLight = await GetTestLightAsync();
			Logger.LogInformation("Using test light: {Label} ({Id})", TestLight.Label, TestLight.Id);

			await OnInitializedAsync();
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Could not resolve the test fixtures");
		}
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		// Restore lights to their original state after all tests in this class complete
		if (OriginalLightStates is null || OriginalLightStates.Count == 0)
		{
			Logger.LogInformation("No original state to restore");
			GC.SuppressFinalize(this);
			return;
		}

		try
		{
			await OnDisposingAsync();

			Logger.LogInformation("Restoring original state for {Count} lights", OriginalLightStates.Count);

			foreach (var originalLight in OriginalLightStates.Where(light => light.IsConnected))
			{
				var restoreRequest = new SetStateRequest
				{
					Power = originalLight.PowerState,
					Color = originalLight.Color?.ToString() ?? "white",
					Brightness = (double)originalLight.Brightness,
					Duration = 1.0 // 1 second transition
				};

				await Client.Lights.SetStateAsync(
					new Selector.LightId(originalLight.Id),
					restoreRequest,
					CancellationToken);
			}

			Logger.LogInformation("Successfully restored original light states");
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Failed to restore original light states");
		}

		GC.SuppressFinalize(this);
	}
}
