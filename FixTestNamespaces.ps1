# Fix test file namespaces
$ErrorActionPreference = "Stop"

Write-Host "Fixing test file namespaces..." -ForegroundColor Green

$workspaceRoot = "C:\Users\DavidBond\source\repos\panoramicdata\Lifx.Api"
$testDir = Join-Path $workspaceRoot "Lifx.Api.Test"

# List of test files that need fixing
$testFiles = @(
    "PowerStateTests.cs",
    "SelectorTests.cs",
    "LifxColorTests.cs",
    "LightsTests.cs",
    "EffectsTests.cs"
)

foreach ($fileName in $testFiles) {
    $filePath = Join-Path $testDir $fileName
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # If file doesn't have namespace declaration, add it after using statements
        if ($content -notmatch 'namespace Lifx\.Api\.Test;') {
            # Find the last using statement
            $lines = $content -split "`r?`n"
            $lastUsingIndex = -1
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match '^using ') {
                    $lastUsingIndex = $i
                }
            }
            
            if ($lastUsingIndex -ge 0) {
                # Insert namespace after last using statement
                $newLines = @()
                for ($i = 0; $i -le $lastUsingIndex; $i++) {
                    $newLines += $lines[$i]
                }
                $newLines += ""
                $newLines += "namespace Lifx.Api.Test;"
                $newLines += ""
                for ($i = $lastUsingIndex + 1; $i -lt $lines.Count; $i++) {
                    $newLines += $lines[$i]
                }
                
                $content = $newLines -join "`r`n"
                Set-Content -Path $filePath -Value $content -NoNewline
                Write-Host "  Fixed: $fileName" -ForegroundColor Cyan
            }
        }
    }
}

# Fix Test.cs to use primary constructor properly
$testClassPath = Join-Path $testDir "Test.cs"
$testContent = @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lifx.Api.Models.Cloud;

namespace Lifx.Api.Test;

public abstract class Test
{
	protected ILogger Logger { get; }
	protected LifxClient Client { get; }

	protected Test(ITestOutputHelper testOutputHelper)
	{
		Logger = LoggerFactory.Create(builder => builder
			.AddProvider(new XunitLoggerProvider(testOutputHelper)))
			.CreateLogger<Test>();

		var settings = GetLifxCloudClientSettings();

		Client = new LifxClient(new LifxClientOptions
		{
			ApiToken = settings.AppToken,
			Logger = Logger,
			IsLanEnabled = false
		});
	}

	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

	private static LifxCloudClientSettings GetLifxCloudClientSettings()
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("../../../appsettings.json", optional: true)
			.AddUserSecrets<Test>()
			.Build();

		var appToken = configuration["AppToken"];

		if (string.IsNullOrEmpty(appToken))
		{
			throw new InvalidOperationException(
				"AppToken not found. Please either:\n" +
				"1. Set it in User Secrets using: dotnet user-secrets set \""AppToken\"" \""your-token-here\""\n" +
				"2. Copy appsettings.example.json to appsettings.json and set the AppToken value\n" +
				"Get your token from https://cloud.lifx.com/settings");
		}

		return new LifxCloudClientSettings
		{
			AppToken = appToken
		};
	}
}

public class LifxCloudClientSettings
{
	public string AppToken { get; set; } = string.Empty;
}
"@

Set-Content -Path $testClassPath -Value $testContent
Write-Host "  Fixed: Test.cs" -ForegroundColor Cyan

Write-Host "`nTest files fixed!" -ForegroundColor Green
