# Update LAN files to use new model namespaces
$ErrorActionPreference = "Stop"

Write-Host "Updating LAN files with new model references..." -ForegroundColor Green

$workspaceRoot = "C:\Users\DavidBond\source\repos\panoramicdata\Lifx.Api"
$projectRoot = Join-Path $workspaceRoot "Lifx.Api"
$lanDir = Join-Path $projectRoot "Lan"

# Files to update
$lanFiles = @(
    "LifxClient.DeviceOperations.cs",
    "LifxClient.Discovery.cs",
    "LifxClient.LightOperations.cs",
    "LifxLanClient.cs",
    "LifxResponses.cs",
    "Utilities.cs"
)

foreach ($fileName in $lanFiles) {
    $filePath = Join-Path $lanDir $fileName
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        $modified = $false
        
        # Add using for Models.Lan if file references Device, LightBulb, Color, or MessageType
        if ($content -match '\b(Device|LightBulb|Color|MessageType)\b' -and 
            $content -notmatch 'using Lifx\.Api\.Models\.Lan;') {
            # Insert after namespace declaration
            if ($content -match '(namespace Lifx\.Api\.Lan;)') {
                $content = $content -replace '(namespace Lifx\.Api\.Lan;)', "`$1`r`n`r`nusing Lifx.Api.Models.Lan;"
                $modified = $true
            }
        }
        
        if ($modified) {
            Write-Host "  Updated: $fileName" -ForegroundColor Cyan
            Set-Content -Path $filePath -Value $content -NoNewline
        }
    }
}

# Update Device.cs to remove duplicate Lan references since it's now in Models.Lan
$devicePath = Join-Path $projectRoot "Models\Lan\Device.cs"
if (Test-Path $devicePath) {
    $content = Get-Content $devicePath -Raw
    
    # Remove duplicate using Microsoft.Extensions.Logging and using System.Net if present
    # These might have been copied from Discovery.cs but aren't needed in Device.cs
    $modified = $false
    
    # Clean up the file
    if ($content -match 'using Microsoft\.Extensions\.Logging;') {
        $content = $content -replace 'using Microsoft\.Extensions\.Logging;\r?\n', ''
        $modified = $true
    }
    if ($content -match 'using System\.Net;') {
        $content = $content -replace 'using System\.Net;\r?\n', ''
        $modified = $true
    }
    
    if ($modified) {
        Write-Host "  Cleaned up Device.cs" -ForegroundColor Cyan
        Set-Content -Path $devicePath -Value $content -NoNewline
    }
}

# Update test files
Write-Host "Updating test files..." -ForegroundColor Yellow
$testDir = Join-Path $workspaceRoot "Lifx.Api.Test"
$testFiles = Get-ChildItem -Path $testDir -Filter "*.cs" -Recurse | 
    Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }

foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Update using statements
    if ($content -match 'using Lifx\.Api\.Cloud\.Models;' -and 
        $content -notmatch 'using Lifx\.Api\.Models\.Cloud;') {
        $content = $content -replace 'using Lifx\.Api\.Cloud\.Models;', 'using Lifx.Api.Models.Cloud;'
        $modified = $true
    }
    
    if ($content -match 'using Lifx\.Api\.Cloud\.Models\.Request;' -and 
        $content -notmatch 'using Lifx\.Api\.Models\.Cloud\.Requests;') {
        $content = $content -replace 'using Lifx\.Api\.Cloud\.Models\.Request;', 'using Lifx.Api.Models.Cloud.Requests;'
        $modified = $true
    }
    
    if ($modified) {
        Write-Host "  Updated: $($file.Name)" -ForegroundColor Gray
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}

Write-Host "`nLAN files update complete!" -ForegroundColor Green
