using System.Text.Json.Serialization;
using Lifx.Api.Serialization;
using static Lifx.Api.Models.Cloud.Selector;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Model object for a Light
/// </summary>
public sealed class Light
{
	public const string ColorCapability = "has_color";
	public const string ColorTemperatureCapability = "has_variable_color_temp";

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
	public Hsbk? Color { get; set; }

	[JsonPropertyName("brightness")]
	[JsonInclude]
	public float Brightness { get; private set; }

	[JsonPropertyName("group")]
	[JsonInclude]
	internal CollectionSpec Group { get; private set; } = new();

	[JsonIgnore]
	public string GroupId => Group.Id;

	[JsonIgnore]
	public string GroupName => Group.Name;

	[JsonPropertyName("location")]
	[JsonInclude]
	internal CollectionSpec Location { get; private set; } = new();

	[JsonIgnore]
	public string LocationId => Location.Id;

	[JsonIgnore]
	public string LocationName => Location.Name;

	[JsonPropertyName("product")]
	[JsonInclude]
	private LightProduct? Product { get; init; }

	[JsonPropertyName("last_seen")]
	[JsonInclude]
	[JsonConverter(typeof(FlexibleDateTimeConverter))]
	public DateTime? LastSeen { get; private set; }

	[JsonPropertyName("seconds_since_seen")]
	[JsonInclude]
	public float SecondsSinceSeen { get; private set; }

	[JsonPropertyName("product_name")]
	[JsonInclude]
	public string ProductName { get; private set; } = string.Empty;

	[JsonPropertyName("capabilities")]
	[JsonInclude]
	[JsonConverter(typeof(CapabilitiesDictionaryConverter))]
	private Dictionary<string, bool>? capabilities { get; init; }

	[JsonIgnore]
	public IEnumerable<string> Capabilities
	{
		get
		{
			if (capabilities is not null)
			{
				foreach (var entry in capabilities)
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
		capabilities is not null && capabilities.ContainsKey(capability) && capabilities[capability];

	public override string ToString() => Label;

	public static implicit operator Selector(Light light) => new LightId(light.Id);
}
