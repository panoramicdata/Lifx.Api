using Lifx.Api.Interfaces;
using Lifx.Api.Lan;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lifx.Api;

/// <summary>
/// Unified LIFX client supporting both Cloud HTTP API and LAN protocol
/// </summary>
public class LifxClient : IDisposable
{
	private readonly ILogger _logger;
	private readonly HttpClient? _httpClient;
	private readonly bool _cloudEnabled;

	private const string BaseUrl = "https://api.lifx.com/v1";

	/// <summary>
	/// Standard JSON serialization options used throughout the LIFX API.
	/// Uses snake_case_lower naming policy for property names and reads enum member values.
	/// </summary>
	public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) },
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	/// <summary>
	/// Cloud HTTP API - Lights operations
	/// </summary>
	public ILifxLightsApi Lights { get; }

	/// <summary>
	/// Cloud HTTP API - Effects operations
	/// </summary>
	public ILifxEffectsApi Effects { get; }

	/// <summary>
	/// Cloud HTTP API - Scenes operations
	/// </summary>
	public ILifxScenesApi Scenes { get; }

	/// <summary>
	/// Cloud HTTP API - Color operations
	/// </summary>
	public ILifxColorApi Color { get; }

	/// <summary>
	/// Products API - Get LIFX product catalog (no API token required)
	/// </summary>
	public ILifxProductsApi Products { get; }

	/// <summary>
	/// LAN Protocol client for direct local network communication (if enabled)
	/// </summary>
	public LifxLanClient? Lan { get; }

	public LifxClient(LifxClientOptions options)
	{
		_logger = options.Logger;
		_cloudEnabled = !string.IsNullOrEmpty(options.ApiToken);

		// Initialize Cloud API clients if token is provided
		if (_cloudEnabled)
		{
			_httpClient = CreateHttpClient(options.ApiToken!);
			Lights = CreateApiClient<ILifxLightsApi>(options.ApiToken!);
			Effects = CreateApiClient<ILifxEffectsApi>(options.ApiToken!);
			Scenes = CreateApiClient<ILifxScenesApi>(options.ApiToken!);
			Color = CreateApiClient<ILifxColorApi>(options.ApiToken!);
		}
		else
		{
			// Create stub clients that throw if used
			Lights = null!;
			Effects = null!;
			Scenes = null!;
			Color = null!;
		}

		// Initialize Products API (no token required, uses GitHub raw URL)
		Products = CreateProductsApiClient();

		// Initialize LAN client if enabled
		if (options.IsLanEnabled)
		{
			Lan = new LifxLanClient(options.Logger);
		}
	}

	/// <summary>
	/// Starts the LAN client for device discovery and communication
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	public void StartLan(CancellationToken cancellationToken)
	{
		if (Lan is null)
		{
			throw new InvalidOperationException("LAN client not enabled. Set IsLanEnabled = true in LifxClientOptions.");
		}

		Lan.Start(cancellationToken);
	}

	/// <summary>
	/// Starts device discovery on the LAN
	/// </summary>
	public void StartDeviceDiscovery(CancellationToken cancellationToken)
	{
		if (Lan is null)
		{
			throw new InvalidOperationException("LAN client not enabled. Set IsLanEnabled = true in LifxClientOptions.");
		}

		Lan.StartDeviceDiscovery(cancellationToken);
	}

	/// <summary>
	/// Stops device discovery on the LAN
	/// </summary>
	public void StopDeviceDiscovery() => Lan?.StopDeviceDiscovery();

	private static T CreateApiClient<T>(string apiToken)
	{
		var httpClient = CreateHttpClient(apiToken);
		return RestService.For<T>(httpClient, new RefitSettings
		{
			ContentSerializer = new SystemTextJsonContentSerializer(JsonSerializerOptions)
		});
	}

	private static HttpClient CreateHttpClient(string apiToken)
	{
		var httpClient = new HttpClient
		{
			BaseAddress = new Uri(BaseUrl)
		};
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
		return httpClient;
	}

	private static ILifxProductsApi CreateProductsApiClient()
	{
		var httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://raw.githubusercontent.com")
		};
		return RestService.For<ILifxProductsApi>(httpClient, new RefitSettings
		{
			ContentSerializer = new SystemTextJsonContentSerializer(JsonSerializerOptions)
		});
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_httpClient?.Dispose();
			Lan?.Dispose();
		}
	}

	~LifxClient()
	{
		Dispose(false);
	}
}
