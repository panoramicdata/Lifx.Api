using Lifx.Api;

namespace Lifx.Cli;

/// <summary>
/// Factory for creating LIFX clients with consistent configuration.
/// </summary>
public class LifxClientFactory
{
	/// <summary>
	/// Creates a Cloud API client using the resolved API token.
	/// </summary>
	/// <param name="tokenOverride">Optional token override from the command line.</param>
	/// <returns>A configured LIFX client.</returns>
	public virtual ILifxClient CreateCloudClient(string? tokenOverride = null)
	{
		var apiToken = ConfigManager.GetApiToken(tokenOverride);
		return new LifxClient(new LifxClientOptions { ApiToken = apiToken });
	}

	/// <summary>
	/// Creates a LAN-only client (no API token required).
	/// </summary>
	/// <returns>A configured LIFX client with LAN enabled.</returns>
	public virtual ILifxClient CreateLanClient()
		=> new LifxClient(new LifxClientOptions { IsLanEnabled = true });

	/// <summary>
	/// Creates a minimal client for token-free APIs (e.g., Products).
	/// </summary>
	/// <returns>A configured LIFX client.</returns>
	public virtual ILifxClient CreateAnonymousClient()
		=> new LifxClient(new LifxClientOptions());
}
