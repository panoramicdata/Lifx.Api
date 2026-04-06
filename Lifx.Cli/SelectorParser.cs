using Lifx.Api.Models.Cloud;
using static Lifx.Api.Models.Cloud.Selector;

namespace Lifx.Cli;

/// <summary>
/// Represents the SelectorParser type.
/// </summary>
public static class SelectorParser
{
	/// <summary>
	/// Performs ParseSelector operation.
	/// </summary>
	public static Selector ParseSelector(string input)
	{
		if (string.IsNullOrWhiteSpace(input) || input.Equals("all", StringComparison.OrdinalIgnoreCase))
		{
			return Selector.All;
		}

		// Try to parse selector format: type:value
		var parts = input.Split(':', 2);
		if (parts.Length == 2)
		{
			var type = parts[0].ToLowerInvariant();
			var value = parts[1];

			return type switch
			{
				"id" or "light_id" => new LightId(value),
				"label" => new LightLabel(value),
				"group" or "group_id" => new GroupId(value),
				"group_label" => new GroupLabel(value),
				"location" or "location_id" => new LocationId(value),
				"location_label" => new LocationLabel(value),
				_ => throw new ArgumentException($"Unknown selector type: {type}")
			};
		}

		// If no colon, treat as label
		return new LightLabel(input);
	}
}
