namespace Lifx.Api.Models.Cloud;

/// <summary>
/// Used for addressing one or many Lights belonging to the authenticated account.
/// </summary>
public class Selector
{
	private const string TYPE_ALL = "all";
	private const string TYPE_RANDOM = "random";
	private const string TYPE_LIGHT_ID = "id";
	private const string TYPE_GROUP_ID = "group_id";
	private const string TYPE_GROUP_LABEL = "group";
	private const string TYPE_LOCATION_ID = "location_id";
	private const string TYPE_LOCATION_LABEL = "location";

	/// <summary>
	/// All lights belonging to the authenticated account.
	/// </summary>
	public static readonly Selector All = new(TYPE_ALL);
	/// <summary>
	/// One randomly selected light belonging to the authenticated account.
	/// </summary>
	private static readonly Selector Random = new(TYPE_RANDOM) { IsSingle = true };

	private readonly string selector;
	internal bool IsSingle { get; private set; }

	private Selector(string selector) { this.selector = selector; }

	private Selector(string type, string criteria) : this(string.Format("{0}:{1}", type, criteria)) { }

	public override string ToString() => selector;

	/// <summary>
	/// Only the light with the given ID.
	/// </summary>
	public class LightId : Selector
	{
		public LightId(string id) : base(TYPE_LIGHT_ID, id) { IsSingle = true; }
	}

	/// <summary>
	/// The first light that matches the label.
	/// </summary>
	public class LightLabel : Selector
	{
		public LightLabel(string label) : base("label", label) { IsSingle = true; }
	}

	/// <summary>
	/// Only the lights belonging to the group with the given ID.
	/// </summary>
	public class GroupId(string id) : Selector(TYPE_GROUP_ID, id)
	{
	}

	/// <summary>
	/// Only the lights belonging to the groups matching the given label.
	/// </summary>
	public class GroupLabel(string label) : Selector(TYPE_GROUP_LABEL, label)
	{
	}


	/// <summary>
	/// OOnly the lights belonging to the location matching the given ID.
	/// </summary>
	public class LocationId(string id) : Selector(TYPE_LOCATION_ID, id)
	{
	}

	/// <summary>
	/// Only the lights belonging to the locations matching the given label.
	/// </summary>
	public class LocationLabel(string label) : Selector(TYPE_LOCATION_LABEL, label)
	{
	}

	public static explicit operator Selector(string selector)
	{
		switch (selector)
		{
			case TYPE_ALL: return All;
			case TYPE_RANDOM: return Random;
			default:
				int criteria = selector.IndexOf(':');
				if (0 <= criteria && criteria < selector.Length - 1)
				{
					string remainder = selector[(criteria + 1)..];
					return (selector[..criteria]) switch
					{
						TYPE_LIGHT_ID => new LightId(remainder),
						TYPE_GROUP_ID => new GroupId(remainder),
						TYPE_GROUP_LABEL => new GroupLabel(remainder),
						TYPE_LOCATION_ID => new LocationId(remainder),
						TYPE_LOCATION_LABEL => new LocationLabel(remainder),
						"label" => new LightLabel(remainder),
						_ => new LightLabel(selector),
					};
				}
				else
				{
					return new LightLabel(selector);
				}
		}
	}
}
