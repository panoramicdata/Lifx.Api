using AwesomeAssertions;
using Lifx.Cli;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for CLI ConfigManager token resolution logic.
/// </summary>
[Collection("Unit Tests")]
public class ConfigManagerTests
{
	/// <summary>
	/// Tests that override token takes highest priority.
	/// </summary>
	[Fact]
	public void TryGetApiToken_WithOverride_Returns_Override()
	{
		var result = ConfigManager.TryGetApiToken("my-override-token");
		result.Should().Be("my-override-token");
	}

	/// <summary>
	/// Tests that whitespace override is ignored.
	/// </summary>
	[Fact]
	public void TryGetApiToken_WhitespaceOverride_Falls_Through()
	{
		// With whitespace override and no env var or stored token,
		// should return null (falls through all priorities)
		var originalEnv = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		try
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", null);
			var result = ConfigManager.TryGetApiToken("   ");
			// Result depends on whether a stored credential exists;
			// we just verify it doesn't return the whitespace string
			result.Should().NotBe("   ");
		}
		finally
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", originalEnv);
		}
	}

	/// <summary>
	/// Tests that environment variable is used when no override provided.
	/// </summary>
	[Fact]
	public void TryGetApiToken_EnvironmentVariable_Used_When_No_Override()
	{
		var originalEnv = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		try
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", "env-token-value");
			var result = ConfigManager.TryGetApiToken();
			result.Should().Be("env-token-value");
		}
		finally
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", originalEnv);
		}
	}

	/// <summary>
	/// Tests that override token takes priority over environment variable.
	/// </summary>
	[Fact]
	public void TryGetApiToken_Override_Beats_EnvironmentVariable()
	{
		var originalEnv = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		try
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", "env-token");
			var result = ConfigManager.TryGetApiToken("override-token");
			result.Should().Be("override-token");
		}
		finally
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", originalEnv);
		}
	}

	/// <summary>
	/// Tests that GetApiToken throws when no token is available.
	/// </summary>
	[Fact]
	public void GetApiToken_NoToken_Throws_InvalidOperationException()
	{
		var originalEnv = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		try
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", null);

			// Only throws if no stored credential exists either
			// If this test runs in an environment with stored credentials, it won't throw
			if (!SecureCredentialManager.HasStoredToken())
			{
				var act = () => ConfigManager.GetApiToken();
				act.Should().Throw<InvalidOperationException>()
					.WithMessage("*No LIFX Cloud API token configured*");
			}
		}
		finally
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", originalEnv);
		}
	}

	/// <summary>
	/// Tests that GetApiToken returns token when override provided.
	/// </summary>
	[Fact]
	public void GetApiToken_WithOverride_Returns_Token()
	{
		var result = ConfigManager.GetApiToken("valid-token");
		result.Should().Be("valid-token");
	}

	/// <summary>
	/// Tests that CliConfiguration has expected defaults.
	/// </summary>
	[Fact]
	public void CliConfiguration_Defaults()
	{
		var config = new CliConfiguration();
		config.UseLan.Should().BeTrue();
		config.DefaultDuration.Should().Be(1000);
		config.DefaultSelector.Should().Be("all");
	}
}
