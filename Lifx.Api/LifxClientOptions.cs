using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lifx.Api;

/// <summary>
/// Represents the LifxClientOptions type.
/// </summary>
public class LifxClientOptions
{
	/// <summary>
	/// Gets or sets ApiToken.
	/// </summary>
	public string? ApiToken { get; set; }

	/// <summary>
	/// Gets or sets Logger.
	/// </summary>
	public ILogger Logger { get; set; } = NullLogger.Instance;

	/// <summary>
	/// Gets or sets IsLanEnabled.
	/// </summary>
	public bool IsLanEnabled { get; set; }

	/// <summary>
	/// Gets or sets whether the client refuses every request that is not a read (GET, HEAD or OPTIONS).
	/// </summary>
	public bool IsReadOnly { get; set; }
}
