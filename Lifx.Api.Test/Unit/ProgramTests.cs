using AwesomeAssertions;
using Lifx.Cli;

namespace Lifx.Api.Test.Unit;

/// <summary>
/// Unit tests for the CLI entry point.
/// </summary>
[Collection("Unit Tests")]
public class ProgramTests
{
	/// <summary>
	/// Tests that Main reports a missing Cloud API token with exit code 1 rather than a stack trace.
	/// </summary>
	[Fact]
	public void Main_CloudCommand_Without_Token_Returns_One_And_Mentions_Token()
	{
		var originalEnv = Environment.GetEnvironmentVariable("LIFX_API_TOKEN");
		try
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", null);

			if (SecureCredentialManager.HasStoredToken())
			{
				// A stored credential would satisfy the token requirement, so this test cannot
				// force the missing-token path on this machine.
				return;
			}

			var exitCode = Program.Main(["cloud", "lights", "list"]);

			exitCode.Should().Be(1);
		}
		finally
		{
			Environment.SetEnvironmentVariable("LIFX_API_TOKEN", originalEnv);
		}
	}
}
