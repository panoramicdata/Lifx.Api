using Lifx.Api.Interfaces;
using Lifx.Api.Lan;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Lifx.Api;

/// <summary>
/// Represents the LifxClient type.
/// </summary>
public class LifxClient : IDisposable
{
	private readonly ILogger _logger;
	private readonly HttpClient? _httpClient;
	private readonly bool _cloudEnabled;

	private const string BaseUrl = "https://api.lifx.com/v1";

	/// <summary>
	/// Gets or sets JsonSerializerOptions.
	/// </summary>
	public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) },
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		TypeInfoResolver = new DefaultJsonTypeInfoResolver()
	};

	/// <summary>
	/// Gets or sets Lights.
	/// </summary>
	public ILifxLightsApi Lights { get; }

	/// <summary>
	/// Gets or sets Effects.
	/// </summary>
	public ILifxEffectsApi Effects { get; }

	/// <summary>
	/// Gets or sets Scenes.
	/// </summary>
	public ILifxScenesApi Scenes { get; }

	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	public ILifxColorApi Color { get; }

	/// <summary>
	/// Gets or sets Products.
	/// </summary>
	public ILifxProductsApi Products { get; }

	/// <summary>
	/// Gets or sets Lan.
	/// </summary>
	public LifxLanClient? Lan { get; }

	/// <summary>
	/// Represents a public API member.
	/// </summary>
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
	/// Performs StartLan operation.
	/// </summary>
	public void StartLan(CancellationToken cancellationToken)
	{
		if (Lan is null)
		{
			throw new InvalidOperationException("LAN client not enabled. Set IsLanEnabled = true in LifxClientOptions.");
		}

		Lan.Start(cancellationToken);
	}

	/// <summary>
	/// Performs StartDeviceDiscovery operation.
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
	/// Performs StopDeviceDiscovery operation.
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

	/// <summary>
	/// Performs Dispose operation.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases resources used by this instance.
	/// </summary>
	/// <param name="disposing"><c>true</c> when called from <see cref="Dispose()"/>; otherwise <c>false</c>.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_httpClient?.Dispose();
			Lan?.Dispose();
		}
	}

	/// <summary>
	/// Finalizes an instance of the <see cref="LifxClient"/> class.
	/// </summary>
	~LifxClient()
	{
		Dispose(false);
	}
}
