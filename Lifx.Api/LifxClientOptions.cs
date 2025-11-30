using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lifx.Api;

/// <summary>
/// Configuration options for the LIFX client
/// </summary>
public class LifxClientOptions
{
	/// <summary>
	/// Required LIFX Cloud API token for HTTP API access. Get your token from https://cloud.lifx.com/settings
	/// Leave null if only using LAN protocol.
	/// </summary>
	public string? ApiToken { get; set; }

	/// <summary>
	/// Optional logger for diagnostic information. Defaults to NullLogger if not provided.
	/// </summary>
	public ILogger Logger { get; set; } = NullLogger.Instance;

	/// <summary>
	/// Enable LAN protocol for direct local network communication. Default is false.
	/// </summary>
	public bool IsLanEnabled { get; set; } = false;
}
