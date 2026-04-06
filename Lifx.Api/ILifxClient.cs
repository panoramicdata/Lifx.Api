using Lifx.Api.Interfaces;
using Lifx.Api.Lan;

namespace Lifx.Api;

/// <summary>
/// Interface for the LIFX client, providing access to Cloud and LAN APIs.
/// </summary>
public interface ILifxClient : IDisposable
{
	/// <summary>
	/// Gets the Lights API for controlling light power, color, and state.
	/// </summary>
	ILifxLightsApi Lights { get; }

	/// <summary>
	/// Gets the Effects API for running visual effects on lights.
	/// </summary>
	ILifxEffectsApi Effects { get; }

	/// <summary>
	/// Gets the Scenes API for listing and activating scenes.
	/// </summary>
	ILifxScenesApi Scenes { get; }

	/// <summary>
	/// Gets the Color API for validating and converting colors.
	/// </summary>
	ILifxColorApi Color { get; }

	/// <summary>
	/// Gets the Products API for querying the LIFX product catalog.
	/// </summary>
	ILifxProductsApi Products { get; }

	/// <summary>
	/// Gets the LAN client for local network device control, or null if LAN is not enabled.
	/// </summary>
	LifxLanClient? Lan { get; }

	/// <summary>
	/// Starts the LAN client for local network communication.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token.</param>
	void StartLan(CancellationToken cancellationToken);

	/// <summary>
	/// Starts discovering LIFX devices on the local network.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token.</param>
	void StartDeviceDiscovery(CancellationToken cancellationToken);

	/// <summary>
	/// Stops discovering LIFX devices on the local network.
	/// </summary>
	void StopDeviceDiscovery();
}
