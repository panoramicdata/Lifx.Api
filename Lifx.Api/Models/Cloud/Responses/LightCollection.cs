using System.Collections;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the LightCollection type.
/// </summary>
public abstract class LightCollection : IEnumerable<Light>
{
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	public string Id { get; private set; }
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	public string Label { get; private set; }

	/// <summary>
	/// Gets or sets IsOn.
	/// </summary>
	public bool IsOn { get { return lights.Any(l => l.IsOn); } }

	private readonly List<Light> lights;

	internal LightCollection(string id, string label, List<Light> lights)
	{
		Id = id;
		Label = label;
		this.lights = lights;
	}

	/// <summary>
	/// Performs GetEnumerator operation.
	/// </summary>
	public IEnumerator<Light> GetEnumerator() => lights.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => lights.GetEnumerator();

	/// <summary>
	/// Performs ToSelector operation.
	/// </summary>
	public abstract Selector ToSelector();

	/// <summary>
	/// Performs ToString operation.
	/// </summary>
	public override string ToString() => Label;

	/// <summary>
	/// Converts between supported types.
	/// </summary>
	public static implicit operator Selector(LightCollection lightCollection)
	{
		return lightCollection.ToSelector();
	}
}
