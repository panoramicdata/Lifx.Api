using AwesomeAssertions;
using Lifx.Api.Models.Cloud;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Credential-free tests for <see cref="LifxClientOptions.IsReadOnly"/>.
/// </summary>
[Collection("Unit Tests")]
public class ReadOnlyOptionTests
{
	private sealed class StubHandler : HttpMessageHandler
	{
		public int SendCount { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			SendCount++;
			return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
		}
	}

	/// <summary>Verifies the behaviour named by the test.</summary>
	[Fact]
	public void IsReadOnly_DefaultsToFalse()
		=> new LifxClientOptions().IsReadOnly.Should().BeFalse();

	/// <summary>Verifies the behaviour named by the test.</summary>
	[Theory]
	[InlineData("POST")]
	[InlineData("PUT")]
	[InlineData("DELETE")]
	[InlineData("PATCH")]
	public async Task Handler_WhenReadOnly_ThrowsBeforeSendingWrite(string method)
	{
		var stub = new StubHandler();
		using var invoker = new HttpMessageInvoker(new ReadOnlyGuardHandler(new LifxClientOptions { IsReadOnly = true }, stub));

		var act = () => invoker.SendAsync(new HttpRequestMessage(new HttpMethod(method), "https://example.invalid/x"), CancellationToken.None);

		await act.Should().ThrowAsync<InvalidOperationException>();
		stub.SendCount.Should().Be(0);
	}

	/// <summary>Verifies the behaviour named by the test.</summary>
	[Fact]
	public async Task Handler_WhenReadOnly_AllowsGet()
	{
		var stub = new StubHandler();
		using var invoker = new HttpMessageInvoker(new ReadOnlyGuardHandler(new LifxClientOptions { IsReadOnly = true }, stub));

		var response = await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.invalid/x"), CancellationToken.None);

		response.IsSuccessStatusCode.Should().BeTrue();
		stub.SendCount.Should().Be(1);
	}

	/// <summary>Verifies the behaviour named by the test.</summary>
	[Fact]
	public async Task Handler_ReadsIsReadOnlyOnEachRequest()
	{
		var options = new LifxClientOptions();
		var stub = new StubHandler();
		using var invoker = new HttpMessageInvoker(new ReadOnlyGuardHandler(options, stub));

		await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://example.invalid/x"), CancellationToken.None);
		options.IsReadOnly = true;
		var act = () => invoker.SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://example.invalid/x"), CancellationToken.None);

		await act.Should().ThrowAsync<InvalidOperationException>();
		stub.SendCount.Should().Be(1);
	}

	/// <summary>Verifies the behaviour named by the test.</summary>
	[Fact]
	public async Task Client_WhenReadOnly_RefusesCloudWriteWithoutNetwork()
	{
		using var client = new LifxClient(new LifxClientOptions { ApiToken = "not-a-real-token", IsReadOnly = true });

		var act = () => client.Lights.TogglePowerAsync(Selector.All, new TogglePowerRequest(), CancellationToken.None);

		(await act.Should().ThrowAsync<Refit.ApiRequestException>()).WithInnerException<InvalidOperationException>();
	}
}
