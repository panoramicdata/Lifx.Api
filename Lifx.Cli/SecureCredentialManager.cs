using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Lifx.Cli;

/// <summary>
/// Manages secure storage of LIFX API credentials
/// - Windows: Uses Data Protection API (DPAPI) for encryption
/// - Non-Windows: Stores in ~/.lifx/credentials.json (file permissions based security)
/// </summary>
public static class SecureCredentialManager
{
	private static readonly string CredentialDirectory = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
		".lifx");

	private static readonly string WindowsCredentialFile = Path.Combine(CredentialDirectory, "credentials.dat");
	private static readonly string CrossPlatformCredentialFile = Path.Combine(CredentialDirectory, "credentials.json");

	private static bool IsWindows => OperatingSystem.IsWindows();

	/// <summary>
	/// Stores the API token securely
	/// </summary>
	public static bool StoreApiToken(string apiToken)
	{
		if (string.IsNullOrWhiteSpace(apiToken))
		{
			return false;
		}

		try
		{
			// Ensure directory exists
			if (!Directory.Exists(CredentialDirectory))
			{
				Directory.CreateDirectory(CredentialDirectory);
			}

			if (IsWindows)
			{
				return StoreApiTokenWindows(apiToken);
			}
			else
			{
				return StoreApiTokenCrossPlatform(apiToken);
			}
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Retrieves the API token from storage
	/// </summary>
	public static string? GetApiToken()
	{
		try
		{
			if (IsWindows && File.Exists(WindowsCredentialFile))
			{
				return GetApiTokenWindows();
			}
			else if (File.Exists(CrossPlatformCredentialFile))
			{
				return GetApiTokenCrossPlatform();
			}

			return null;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Removes the API token from storage
	/// </summary>
	public static bool DeleteApiToken()
	{
		try
		{
			var deleted = false;

			if (File.Exists(WindowsCredentialFile))
			{
				File.Delete(WindowsCredentialFile);
				deleted = true;
			}

			if (File.Exists(CrossPlatformCredentialFile))
			{
				File.Delete(CrossPlatformCredentialFile);
				deleted = true;
			}

			return deleted;
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
		return File.Exists(WindowsCredentialFile) || File.Exists(CrossPlatformCredentialFile);
	}

	/// <summary>
	/// Gets a description of where credentials are stored
	/// </summary>
	public static string GetStorageLocation()
	{
		if (IsWindows && File.Exists(WindowsCredentialFile))
		{
			return "~/.lifx/credentials.dat (DPAPI encrypted)";
		}
		else if (File.Exists(CrossPlatformCredentialFile))
		{
			return "~/.lifx/credentials.json (file permissions protected)";
		}

		return IsWindows 
			? "~/.lifx/credentials.dat (DPAPI encrypted)" 
			: "~/.lifx/credentials.json (file permissions protected)";
	}

	#region Windows-specific (DPAPI)

	[SupportedOSPlatform("windows")]
	private static bool StoreApiTokenWindows(string apiToken)
	{
		// Encrypt the token using DPAPI (Windows only, user-specific)
		var plainBytes = Encoding.UTF8.GetBytes(apiToken);
		var encryptedBytes = ProtectedData.Protect(
			plainBytes,
			optionalEntropy: null,
			scope: DataProtectionScope.CurrentUser);

		// Save to file
		File.WriteAllBytes(WindowsCredentialFile, encryptedBytes);
		return true;
	}

	[SupportedOSPlatform("windows")]
	private static string? GetApiTokenWindows()
	{
		// Read encrypted data
		var encryptedBytes = File.ReadAllBytes(WindowsCredentialFile);

		// Decrypt using DPAPI
		var plainBytes = ProtectedData.Unprotect(
			encryptedBytes,
			optionalEntropy: null,
			scope: DataProtectionScope.CurrentUser);

		return Encoding.UTF8.GetString(plainBytes);
	}

	#endregion

	#region Cross-platform (JSON file with permissions)

	private static bool StoreApiTokenCrossPlatform(string apiToken)
	{
		var credentialData = new CredentialFile
		{
			ApiToken = apiToken,
			CreatedAt = DateTime.UtcNow,
			Platform = Environment.OSVersion.Platform.ToString()
		};

		var json = JsonSerializer.Serialize(credentialData, new JsonSerializerOptions
		{
			WriteIndented = true
		});

		File.WriteAllText(CrossPlatformCredentialFile, json);

		// On Unix-like systems, set file permissions to user-only (600)
		if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
		{
			try
			{
				// Set permissions to 600 (user read/write only)
				File.SetUnixFileMode(CrossPlatformCredentialFile, 
					UnixFileMode.UserRead | UnixFileMode.UserWrite);
			}
			catch
			{
				// Ignore if setting permissions fails
			}
		}

		return true;
	}

	private static string? GetApiTokenCrossPlatform()
	{
		var json = File.ReadAllText(CrossPlatformCredentialFile);
		var credentialData = JsonSerializer.Deserialize<CredentialFile>(json);
		return credentialData?.ApiToken;
	}

	private class CredentialFile
	{
		public string ApiToken { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public string Platform { get; set; } = string.Empty;
	}

	#endregion
}
