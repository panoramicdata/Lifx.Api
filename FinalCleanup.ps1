# Final cleanup script to complete model reorganization
$ErrorActionPreference = "Stop"

Write-Host "Starting final cleanup..." -ForegroundColor Green

$workspaceRoot = "C:\Users\DavidBond\source\repos\panoramicdata\Lifx.Api"
$projectRoot = Join-Path $workspaceRoot "Lifx.Api"

# 1. Extract Device class from Discovery.cs and create Device.cs in Models/Lan
Write-Host "Extracting Device class to Models/Lan/Device.cs..." -ForegroundColor Yellow

$discoveryPath = Join-Path $projectRoot "Lan\LifxClient.Discovery.cs"
$discoveryContent = Get-Content $discoveryPath -Raw

# Extract the Device class definition
$deviceClassPattern = '(?s)/// <summary>\r?\n/// LIFX Generic Device\r?\n/// </summary>\r?\npublic abstract class Device\s*\{.*?\r?\n\}'
if ($discoveryContent -match $deviceClassPattern) {
    $deviceClass = $matches[0]
    
    # Create Device.cs in Models/Lan
    $deviceContent = @"
namespace Lifx.Api.Models.Lan;

$deviceClass
"@
    
    $devicePath = Join-Path $projectRoot "Models\Lan\Device.cs"
    Set-Content -Path $devicePath -Value $deviceContent
    Write-Host "  Created Models/Lan/Device.cs" -ForegroundColor Cyan
    
    # Remove Device class from Discovery.cs
    $discoveryContent = $discoveryContent -replace $deviceClassPattern, ''
    # Clean up extra blank lines
    $discoveryContent = $discoveryContent -replace '\r?\n\r?\n\r?\n+', "`r`n`r`n"
    Set-Content -Path $discoveryPath -Value $discoveryContent -NoNewline
    Write-Host "  Removed Device class from Discovery.cs" -ForegroundColor Cyan
}

# 2. Update all LAN files to remove Models.Lan prefix where using directive exists
Write-Host "Simplifying type references in LAN files..." -ForegroundColor Yellow

$lanFiles = @(
    "Lan\LifxClient.Discovery.cs",
    "Lan\LifxClient.DeviceOperations.cs",
    "Lan\LifxClient.LightOperations.cs"
)

foreach ($relPath in $lanFiles) {
    $filePath = Join-Path $projectRoot $relPath
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Since we have "using Lifx.Api.Models.Lan;" we can simplify the types
        $content = $content -replace 'Models\.Lan\.Device', 'Device'
        $content = $content -replace 'Models\.Lan\.LightBulb', 'LightBulb'
        $content = $content -replace 'Models\.Lan\.Color', 'Color'
        $content = $content -replace 'Models\.Lan\.MessageType', 'MessageType'
        
        Set-Content -Path $filePath -Value $content -NoNewline
        Write-Host "  Simplified: $relPath" -ForegroundColor Cyan
    }
}

# 3. Update test files to use correct namespaces
Write-Host "Fixing test project namespaces..." -ForegroundColor Yellow

$testDir = Join-Path $workspaceRoot "Lifx.Api.Test"
$testFiles = Get-ChildItem -Path $testDir -Filter "*.cs" -File | Where-Object { $_.Name -ne "GlobalUsings.cs" }

foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Ensure proper using statements
    if ($content -match 'namespace Lifx\.Api\.Test;' -and $content -notmatch 'using Lifx\.Api\.Models\.Cloud;') {
        # Add missing using after namespace
        $content = $content -replace '(namespace Lifx\.Api\.Test;)', "$1`r`nusing Lifx.Api.Models.Cloud;"
        $modified = $true
    }
    
    if ($content -match 'namespace Lifx\.Api\.Test;' -and $content -notmatch 'using Lifx\.Api\.Models\.Cloud\.Requests;') {
        # Add missing using after namespace
        $content = $content -replace '(namespace Lifx\.Api\.Test;)', "$1`r`nusing Lifx.Api.Models.Cloud.Requests;"
        $modified = $true
    }
    
    if ($modified) {
        # Clean up duplicate namespace declarations if any
        $lines = $content -split "`r?`n"
        $cleanedLines = @()
        $seenNamespace = $false
        foreach ($line in $lines) {
            if ($line -match '^namespace Lifx\.Api\.Test;') {
                if (-not $seenNamespace) {
                    $cleanedLines += $line
                    $seenNamespace = $true
                }
            } else {
                $cleanedLines += $line
            }
        }
        $content = $cleanedLines -join "`r`n"
        
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Fixed: $($file.Name)" -ForegroundColor Cyan
    }
}

# 4. Clean up old empty directories
Write-Host "Cleaning up empty directories..." -ForegroundColor Yellow

$dirsToCheck = @(
    "Cloud\Models\Request",
    "Cloud\Models\Response",
    "Cloud\Models",
    "Cloud"
)

foreach ($dir in $dirsToCheck) {
    $dirPath = Join-Path $projectRoot $dir
    if ((Test-Path $dirPath) -and ((Get-ChildItem $dirPath -Recurse).Count -eq 0)) {
        Remove-Item $dirPath -Recurse -Force
        Write-Host "  Removed: $dir" -ForegroundColor Gray
    }
}

# 5. Update Extensions.cs namespace if it still references old paths
$extensionsPath = Join-Path $projectRoot "Cloud\Extensions.cs"
if (Test-Path $extensionsPath) {
    $content = Get-Content $extensionsPath -Raw
    $modified = $false
    
    if ($content -notmatch 'using Lifx\.Api\.Models\.Cloud;') {
        $content = $content -replace '(namespace Lifx\.Api\.Cloud;)', "using Lifx.Api.Models.Cloud;`r`nusing Lifx.Api.Models.Cloud.Responses;`r`n`r`n$1"
        $modified = $true
    }
    
    if ($modified) {
        Set-Content -Path $extensionsPath -Value $content -NoNewline
        Write-Host "  Updated Extensions.cs" -ForegroundColor Cyan
    }
}

# 6. Create a global usings file for the test project if it doesn't exist
$testGlobalUsingsPath = Join-Path $testDir "GlobalUsings.cs"
if (-not (Test-Path $testGlobalUsingsPath)) {
    $globalUsingsContent = @"
global using Xunit;
global using Xunit.Abstractions;
global using Lifx.Api;
global using Lifx.Api.Models.Cloud;
global using Lifx.Api.Models.Cloud.Requests;
global using Lifx.Api.Models.Cloud.Responses;
"@
    Set-Content -Path $testGlobalUsingsPath -Value $globalUsingsContent
    Write-Host "  Created GlobalUsings.cs for test project" -ForegroundColor Cyan
}

Write-Host "`nFinal cleanup complete!" -ForegroundColor Green
Write-Host "`nSummary of changes:" -ForegroundColor Yellow
Write-Host "  ? Extracted Device class to Models/Lan/Device.cs" -ForegroundColor White
Write-Host "  ? Simplified type references in LAN files" -ForegroundColor White
Write-Host "  ? Fixed test project namespace references" -ForegroundColor White
Write-Host "  ? Cleaned up empty directories" -ForegroundColor White
Write-Host "  ? Updated Extensions.cs" -ForegroundColor White
Write-Host "  ? Created GlobalUsings.cs for tests" -ForegroundColor White
Write-Host "`nPlease rebuild the solution now." -ForegroundColor Cyan
