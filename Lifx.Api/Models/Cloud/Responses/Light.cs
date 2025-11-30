using Lifx.Api.Extensions;
using Lifx.Api.Interfaces;
using Lifx.Api.Models.Cloud.Requests;
using System.Text.Json.Serialization;
using static Lifx.Api.Models.Cloud.Selector;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Model object for a Light
/// </summary>
public sealed class Light : ILightTarget<ApiResponse>
{
	public const string ColorCapability = "has_color";
	public const string ColorTemperatureCapability = "has_variable_color_temp";

	[JsonIgnore]
	internal LifxClient? Client { get; set; }

	/// <summary>
	/// Serial number of the light
	/// </summary>
	[JsonPropertyName("id")]
	[JsonInclude]
	public string Id { get; private set; } = string.Empty;

	[JsonPropertyName("uuid")]
	[JsonInclude]
	public string Uuid { get; private set; } = string.Empty;

	[JsonPropertyName("label")]
	[JsonInclude]
	public string Label { get; private set; } = string.Empty;

	[JsonPropertyName("connected")]
	[JsonInclude]
	public bool IsConnected { get; private set; }

	[JsonIgnore]
	public bool IsOn => PowerState == PowerState.On;

	[JsonPropertyName("power")]
	public PowerState PowerState { get; private set; }

	[JsonPropertyName("color")]
	[JsonInclude]
	public Hsbk? Color
	{
		get
		{
			if (color is null)
			{
				return null;
			}

			return new()
			{
				Hue = color.Hue,
				Saturation = color.Saturation,
				Brightness = color.Brightness,
				Kelvin = color.Kelvin
			};
		}
		private set => color = value;
	}

	[JsonPropertyName("brightness")]
	[JsonInclude]
	public float Brightness { get; private set; }

	[JsonPropertyName("group")]
	[JsonInclude]
	internal CollectionSpec Group { get; private set; } = new();

	[JsonIgnore]
	public string GroupId => Group.id;

	[JsonIgnore]
	public string GroupName => Group.name;

	[JsonPropertyName("location")]
	[JsonInclude]
	internal CollectionSpec Location { get; private set; } = new();

	[JsonIgnore]
	public string LocationId => Location.id;

	[JsonIgnore]
	public string LocationName => Location.name;

	[JsonPropertyName("last_seen")]
	[JsonInclude]
	public DateTime? LastSeen { get; private set; }

	[JsonPropertyName("seconds_since_seen")]
	[JsonInclude]
	public float SecondsSinceSeen { get; private set; }

	[JsonPropertyName("product_name")]
	[JsonInclude]
	public string ProductName { get; private set; } = string.Empty;

	[JsonPropertyName("capabilities")]
	[JsonInclude]
	private Dictionary<string, bool>? _capabilities { get; init; }

	private Hsbk? color;

	[JsonIgnore]
	public IEnumerable<string> Capabilities
	{
		get
		{
			if (_capabilities is not null)
			{
				foreach (var entry in _capabilities)
				{
					if (entry.Value)
					{
						yield return entry.Key;
					}
				}
			}
		}
	}

	public bool HasCapability(string capability) =>
		_capabilities is not null && _capabilities.ContainsKey(capability) && _capabilities[capability];

	public async Task<ApiResponse> TogglePower(TogglePowerRequest request)
	{
		if (Client is null)
		{
			throw new InvalidOperationException("Client not initialized");
		}

		return await Client.Lights.TogglePowerAsync((Selector)this, request, CancellationToken.None);
	}

	public async Task<ApiResponse> SetState(SetStateRequest request)
	{
		if (Client is null)
		{
			return new ApiResponse();
		}

		return await Client.Lights.SetStateAsync((Selector)this, request, CancellationToken.None);
	}

	/// <summary>
	/// Re-requests light information
	/// </summary>
	/// <returns>A new instance of this light returned from API</returns>
	public async Task<Light> GetRefreshed()
	{
		if (Client is null)
		{
			throw new InvalidOperationException("Client not initialized");
		}

		return (await Client.Lights.ListAsync((Selector)this, CancellationToken.None)).First();
	}

	/// <summary>
	/// Re-requests light information and updates all properties
	/// </summary>
	public async Task Refresh()
	{
		Light light = await GetRefreshed();
		Id = light.Id;
		Uuid = light.Uuid;
		Label = light.Label;
		IsConnected = light.IsConnected;
		PowerState = light.PowerState;
		Color = light.Color;
		Brightness = light.Brightness;
		Group = light.Group;
		Location = light.Location;
		LastSeen = light.LastSeen;
		SecondsSinceSeen = light.SecondsSinceSeen;
		ProductName = light.ProductName;
	}

	public override string ToString() => Label;

	public static implicit operator Selector(Light light) => new LightId(light.Id);
}
