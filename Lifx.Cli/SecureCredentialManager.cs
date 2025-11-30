using CredentialManagement;

namespace Lifx.Cli;

/// <summary>
/// Manages secure storage of LIFX API credentials using Windows Credential Manager
/// </summary>
public static class SecureCredentialManager
{
	private const string CredentialTarget = "LIFX_CLI_API_TOKEN";
	private const string CredentialDescription = "LIFX Cloud API Token";

	/// <summary>
	/// Stores the API token securely in Windows Credential Manager
	/// </summary>
	public static bool StoreApiToken(string apiToken)
	{
		if (string.IsNullOrWhiteSpace(apiToken))
		{
			return false;
		}

		try
		{
			using var credential = new Credential
			{
				Target = CredentialTarget,
				Username = "LIFX_API",
				Password = apiToken,
				Type = CredentialType.Generic,
				PersistanceType = PersistanceType.LocalComputer,
				Description = CredentialDescription
			};

			return credential.Save();
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Retrieves the API token from Windows Credential Manager
	/// </summary>
	public static string? GetApiToken()
	{
		try
		{
			using var credential = new Credential
			{
				Target = CredentialTarget,
				Type = CredentialType.Generic
			};

			return credential.Load() ? credential.Password : null;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Removes the API token from Windows Credential Manager
	/// </summary>
	public static bool DeleteApiToken()
	{
		try
		{
			using var credential = new Credential
			{
				Target = CredentialTarget,
				Type = CredentialType.Generic
			};

			return credential.Delete();
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Checks if an API token is stored
	/// </summary>
	public static bool HasStoredToken()
	{
		try
		{
			using var credential = new Credential
			{
				Target = CredentialTarget,
				Type = CredentialType.Generic
			};

			return credential.Exists();
		}
		catch
		{
			return false;
		}
	}
}
