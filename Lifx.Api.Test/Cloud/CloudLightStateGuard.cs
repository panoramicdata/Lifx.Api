using Lifx.Api.Extensions;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Api.Test.Cloud;

/// <summary>
/// Records every light's state once before the run and puts it back once at the end.
/// </summary>
/// <remarks>
/// <para>
/// The per-class restore in <see cref="CloudLightStateTest"/> is best effort: it runs inside a
/// try/catch so a failure there cannot fail a passing test, which means a rate limit (HTTP 429)
/// during teardown leaves the lights wherever the tests left them. That has happened.
/// </para>
/// <para>
/// This is the backstop - the run's finally. It captures before any test has touched anything, so
/// what it holds is the genuine pre-run state rather than whatever a previous class left behind,
/// and it restores after everything has finished. If the lights cannot be read (no AppToken on CI,
/// for instance) it does nothing at all.
/// </para>
/// </remarks>
public sealed class CloudLightStateGuard : IAsyncLifetime
{
	private LifxClient? _client;
	private List<Light>? _originalStates;

	/// <summary>
	/// Captures the state of every light before the first test runs.
	/// </summary>
	public async ValueTask InitializeAsync()
	{
		var configuration = Test.TryGetTestConfiguration();
		if (configuration is null)
		{
			// No cloud credentials: there is nothing to protect.
			return;
		}

		try
		{
			_client = new LifxClient(new LifxClientOptions { ApiToken = configuration.AppToken });
			_originalStates = await _client.Lights.ListAsync(Selector.All, CancellationToken.None);
			TestContext.Current.SendDiagnosticMessage(
				$"CloudLightStateGuard: captured {_originalStates.Count} light(s) for the run.");
		}
		catch (Exception ex)
		{
			// Never fail the run over the safety net itself.
			TestContext.Current.SendDiagnosticMessage($"Could not capture light states for the run: {ex.Message}");
			_originalStates = null;
		}
	}

	/// <summary>
	/// Puts every light back the way it was, after the last test has finished.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (_client is not null && _originalStates is { Count: > 0 })
		{
			var restored = _originalStates.Where(light => light.IsConnected).ToList();
			foreach (var light in restored)
			{
				await RestoreAsync(light);
			}

			TestContext.Current.SendDiagnosticMessage(
				$"CloudLightStateGuard: restored {restored.Count} light(s) after the run.");
		}

		_client?.Dispose();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Restores one light, retrying once because a rate limit at the end of a run is exactly the
	/// case this class exists to survive.
	/// </summary>
	private async Task RestoreAsync(Light light)
	{
		var request = new SetStateRequest
		{
			Power = light.PowerState,
			Color = light.Color?.ToString() ?? LifxColor.DefaultWhite,
			Brightness = (double)light.Brightness,
			Duration = 1.0
		};

		for (var attempt = 1; attempt <= 2; attempt++)
		{
			try
			{
				await _client!.Lights.SetStateAsync(
					new Selector.LightId(light.Id),
					request,
					CancellationToken.None);
				return;
			}
			catch (Exception ex) when (attempt == 1)
			{
				TestContext.Current.SendDiagnosticMessage(
					$"Restoring {light.Label} failed ({ex.Message}); retrying once.");
				await Task.Delay(TimeSpan.FromSeconds(2));
			}
			catch (Exception ex)
			{
				TestContext.Current.SendDiagnosticMessage(
					$"Could not restore {light.Label}: {ex.Message}");
				return;
			}
		}
	}
}
