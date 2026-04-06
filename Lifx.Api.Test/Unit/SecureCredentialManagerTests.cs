using AwesomeAssertions;
using Lifx.Cli;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for CLI SecureCredentialManager.
/// </summary>
[Collection("Unit Tests")]
public class SecureCredentialManagerTests
{
	/// <summary>
	/// Tests that StoreApiToken rejects null/whitespace.
	/// </summary>
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void StoreApiToken_InvalidInput_Returns_False(string? token)
	{
		SecureCredentialManager.StoreApiToken(token!).Should().BeFalse();
	}

	/// <summary>
	/// Tests that GetStorageLocation returns a non-empty description.
	/// </summary>
	[Fact]
	public void GetStorageLocation_Returns_NonEmpty_String()
	{
		var location = SecureCredentialManager.GetStorageLocation();
		location.Should().NotBeNullOrWhiteSpace();
		location.Should().Contain("~/.lifx/credentials");
	}

	/// <summary>
	/// Tests that GetStorageLocation describes DPAPI on Windows.
	/// </summary>
	[Fact]
	public void GetStorageLocation_On_Windows_Mentions_DPAPI()
	{
		if (!OperatingSystem.IsWindows())
		{
			return;
		}

		var location = SecureCredentialManager.GetStorageLocation();
		location.Should().Contain("DPAPI");
	}

	/// <summary>
	/// Tests round-trip store and retrieve of API token.
	/// </summary>
	[Fact]
	public void StoreAndRetrieve_ApiToken_RoundTrips()
	{
		// Store original token state to restore later
		var originalToken = SecureCredentialManager.GetApiToken();

		try
		{
			var testToken = $"test-token-{Guid.NewGuid()}";

			var stored = SecureCredentialManager.StoreApiToken(testToken);
			stored.Should().BeTrue();

			SecureCredentialManager.HasStoredToken().Should().BeTrue();

			var retrieved = SecureCredentialManager.GetApiToken();
			retrieved.Should().Be(testToken);
		}
		finally
		{
			// Restore original state
			if (originalToken is not null)
			{
				SecureCredentialManager.StoreApiToken(originalToken);
			}
			else
			{
				SecureCredentialManager.DeleteApiToken();
			}
		}
	}

	/// <summary>
	/// Tests that DeleteApiToken removes stored credentials.
	/// </summary>
	[Fact]
	public void DeleteApiToken_Removes_StoredCredential()
	{
		var originalToken = SecureCredentialManager.GetApiToken();

		try
		{
			// Store a token first
			SecureCredentialManager.StoreApiToken("token-to-delete");
			SecureCredentialManager.HasStoredToken().Should().BeTrue();

			// Delete it
			var deleted = SecureCredentialManager.DeleteApiToken();
			deleted.Should().BeTrue();

			// Verify it's gone
			SecureCredentialManager.HasStoredToken().Should().BeFalse();
			SecureCredentialManager.GetApiToken().Should().BeNull();
		}
		finally
		{
			// Restore original state
			if (originalToken is not null)
			{
				SecureCredentialManager.StoreApiToken(originalToken);
			}
		}
	}

	/// <summary>
	/// Tests that DeleteApiToken returns false when nothing to delete.
	/// </summary>
	[Fact]
	public void DeleteApiToken_WhenNothingStored_Returns_False()
	{
		var originalToken = SecureCredentialManager.GetApiToken();

		try
		{
			// Ensure nothing is stored
			SecureCredentialManager.DeleteApiToken();

			// Second delete should return false
			var result = SecureCredentialManager.DeleteApiToken();
			result.Should().BeFalse();
		}
		finally
		{
			if (originalToken is not null)
			{
				SecureCredentialManager.StoreApiToken(originalToken);
			}
		}
	}
}
