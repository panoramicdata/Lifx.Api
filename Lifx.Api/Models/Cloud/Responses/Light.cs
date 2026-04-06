using System.Text.Json.Serialization;
using Lifx.Api.Serialization;
using static Lifx.Api.Models.Cloud.Selector;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Light type.
/// </summary>
public sealed class Light
{
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public const string ColorCapability = "has_color";
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	public const string ColorTemperatureCapability = "has_variable_color_temp";

	/// <summary>
	/// Serial number of the light
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	[JsonPropertyName("id")]
	[JsonInclude]
	public string Id { get; private set; } = string.Empty;

	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	[JsonPropertyName("uuid")]
	[JsonInclude]
	public string Uuid { get; private set; } = string.Empty;

	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	[JsonPropertyName("label")]
	[JsonInclude]
	public string Label { get; private set; } = string.Empty;

	/// <summary>
	/// Gets or sets IsConnected.
	/// </summary>
	/// <summary>
	/// Gets or sets IsConnected.
	/// </summary>
	/// <summary>
	/// Gets or sets IsConnected.
	/// </summary>
	/// <summary>
	/// Gets or sets IsConnected.
	/// </summary>
	[JsonPropertyName("connected")]
	[JsonInclude]
	public bool IsConnected { get; private set; }

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	[JsonIgnore]
	public bool IsOn => PowerState == PowerState.On;

	/// <summary>
	/// Gets or sets PowerState.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerState.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerState.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerState.
	/// </summary>
	[JsonPropertyName("power")]
	public PowerState PowerState { get; private set; }

	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	[JsonPropertyName("color")]
	public Hsbk? Color { get; set; }

	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	[JsonPropertyName("brightness")]
	[JsonInclude]
	public float Brightness { get; private set; }

	[JsonPropertyName("group")]
	[JsonInclude]
	internal CollectionSpec Group { get; private set; } = new();

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	[JsonIgnore]
	public string GroupId => Group.Id;

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	[JsonIgnore]
	public string GroupName => Group.Name;

	[JsonPropertyName("location")]
	[JsonInclude]
	internal CollectionSpec Location { get; private set; } = new();

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	[JsonIgnore]
	public string LocationId => Location.Id;

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	[JsonIgnore]
	public string LocationName => Location.Name;

	[JsonPropertyName("product")]
	[JsonInclude]
	private LightProduct? Product { get; init; }

	/// <summary>
	/// Gets or sets LastSeen.
	/// </summary>
	/// <summary>
	/// Gets or sets LastSeen.
	/// </summary>
	/// <summary>
	/// Gets or sets LastSeen.
	/// </summary>
	/// <summary>
	/// Gets or sets LastSeen.
	/// </summary>
	[JsonPropertyName("last_seen")]
	[JsonInclude]
	[JsonConverter(typeof(FlexibleDateTimeConverter))]
	public DateTime? LastSeen { get; private set; }

	/// <summary>
	/// Gets or sets SecondsSinceSeen.
	/// </summary>
	/// <summary>
	/// Gets or sets SecondsSinceSeen.
	/// </summary>
	/// <summary>
	/// Gets or sets SecondsSinceSeen.
	/// </summary>
	/// <summary>
	/// Gets or sets SecondsSinceSeen.
	/// </summary>
	[JsonPropertyName("seconds_since_seen")]
	[JsonInclude]
	public float SecondsSinceSeen { get; private set; }

	/// <summary>
	/// Gets or sets ProductName.
	/// </summary>
	/// <summary>
	/// Gets or sets ProductName.
	/// </summary>
	/// <summary>
	/// Gets or sets ProductName.
	/// </summary>
	/// <summary>
	/// Gets or sets ProductName.
	/// </summary>
	[JsonPropertyName("product_name")]
	[JsonInclude]
	public string ProductName { get; private set; } = string.Empty;

	[JsonPropertyName("capabilities")]
	[JsonInclude]
	[JsonConverter(typeof(CapabilitiesDictionaryConverter))]
	private Dictionary<string, bool>? capabilities { get; init; }

	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
	/// <summary>
	/// Represents a public API member.
	/// </summary>
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

	/// <summary>
	/// Performs HasCapability operation.
	/// </summary>
	public bool HasCapability(string capability) =>
		capabilities is not null && capabilities.ContainsKey(capability) && capabilities[capability];

	/// <summary>
	/// Performs ToString operation.
	/// </summary>
	public override string ToString() => Label;

	/// <summary>
	/// Converts between supported types.
	/// </summary>
	public static implicit operator Selector(Light light) => new LightId(light.Id);
}
