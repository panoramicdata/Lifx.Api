using System.Collections;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Interfaces;
using Lifx.Api.Extensions;

namespace Lifx.Api.Models.Cloud.Responses;

public abstract class LightCollection : IEnumerable<Light>, ILightTarget<List<ApiResponse>>
{
	public string Id { get; private set; }
	public string Label { get; private set; }

	public bool IsOn { get { return lights.Any(l => l.IsOn); } }

	private List<Light> lights;
	private readonly LifxClient? client;

	internal LightCollection(LifxClient? client, string id, string label, List<Light> lights)
	{
		this.client = client;
		Id = id;
		Label = label;
		this.lights = lights;
	}

	public IEnumerator<Light> GetEnumerator() => lights.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => lights.GetEnumerator();

	public async Task Refresh()
	{
		if (client is not null)
		{
			lights = await client.Lights.ListAsync(this, CancellationToken.None);
		}
	}

	public abstract Selector ToSelector();

	public override string ToString() => Label;

	public async Task<ApiResponse> TogglePower(TogglePowerRequest request)
	{
		if (client is null) { return new ApiResponse(); }
		return await client.Lights.TogglePowerAsync(this, request, CancellationToken.None);
	}

	public async Task<ApiResponse> SetState(SetStateRequest request)
	{
		if (client is null) { return new ApiResponse(); }
		return await client.Lights.SetStateAsync(this, request, CancellationToken.None);
	}

	public static implicit operator Selector(LightCollection lightCollection)
	{
		return lightCollection.ToSelector();
	}
}
