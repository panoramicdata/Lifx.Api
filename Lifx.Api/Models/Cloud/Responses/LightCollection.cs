using System.Collections;

namespace Lifx.Api.Models.Cloud.Responses;

public abstract class LightCollection : IEnumerable<Light>
{
	public string Id { get; private set; }
	public string Label { get; private set; }

	public bool IsOn { get { return lights.Any(l => l.IsOn); } }

	private readonly List<Light> lights;

	internal LightCollection(string id, string label, List<Light> lights)
	{
		Id = id;
		Label = label;
		this.lights = lights;
	}

	public IEnumerator<Light> GetEnumerator() => lights.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => lights.GetEnumerator();

	public abstract Selector ToSelector();

	public override string ToString() => Label;

	public static implicit operator Selector(LightCollection lightCollection)
	{
		return lightCollection.ToSelector();
	}
}
