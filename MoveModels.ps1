# Model Reorganization Script
# This script moves all Cloud and Lan models to Lifx.Api.Models namespace

$ErrorActionPreference = "Stop"

Write-Host "Starting model reorganization..." -ForegroundColor Green

# Define the workspace root
$workspaceRoot = "C:\Users\DavidBond\source\repos\panoramicdata\Lifx.Api"
$projectRoot = Join-Path $workspaceRoot "Lifx.Api"

# Create new Models directory structure
Write-Host "Creating Models directory structure..." -ForegroundColor Yellow
$modelsRoot = Join-Path $projectRoot "Models"
$modelsCloudRequests = Join-Path $modelsRoot "Cloud\Requests"
$modelsCloudResponses = Join-Path $modelsRoot "Cloud\Responses"
$modelsCloud = Join-Path $modelsRoot "Cloud"
$modelsLan = Join-Path $modelsRoot "Lan"

New-Item -ItemType Directory -Force -Path $modelsCloudRequests | Out-Null
New-Item -ItemType Directory -Force -Path $modelsCloudResponses | Out-Null
New-Item -ItemType Directory -Force -Path $modelsLan | Out-Null

# Define file moves
$fileMoves = @(
    # Cloud Request Models
    @{ Source = "Cloud\Models\Request\ActivateSceneRequest.cs"; Dest = "Models\Cloud\Requests\ActivateSceneRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\BreatheEffectRequest.cs"; Dest = "Models\Cloud\Requests\BreatheEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\CleanRequest.cs"; Dest = "Models\Cloud\Requests\CleanRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\CloudsEffectRequest.cs"; Dest = "Models\Cloud\Requests\CloudsEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\CycleRequest.cs"; Dest = "Models\Cloud\Requests\CycleRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\EffectsOffRequest.cs"; Dest = "Models\Cloud\Requests\EffectsOffRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\FlameEffectRequest.cs"; Dest = "Models\Cloud\Requests\FlameEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\MorphEffectRequest.cs"; Dest = "Models\Cloud\Requests\MorphEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\MoveEffectRequest.cs"; Dest = "Models\Cloud\Requests\MoveEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\PulseEffectRequest.cs"; Dest = "Models\Cloud\Requests\PulseEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\SetStateRequest.cs"; Dest = "Models\Cloud\Requests\SetStateRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\SetStatesRequest.cs"; Dest = "Models\Cloud\Requests\SetStatesRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\StateDeltaRequest.cs"; Dest = "Models\Cloud\Requests\StateDeltaRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\SunriseEffectRequest.cs"; Dest = "Models\Cloud\Requests\SunriseEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\SunsetEffectRequest.cs"; Dest = "Models\Cloud\Requests\SunsetEffectRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    @{ Source = "Cloud\Models\Request\TogglePowerRequest.cs"; Dest = "Models\Cloud\Requests\TogglePowerRequest.cs"; OldNs = "Lifx.Api.Cloud.Models.Request"; NewNs = "Lifx.Api.Models.Cloud.Requests" }
    
    # Cloud Response Models
    @{ Source = "Cloud\Models\Response\ApiResponse.cs"; Dest = "Models\Cloud\Responses\ApiResponse.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\CollectionSpec.cs"; Dest = "Models\Cloud\Responses\CollectionSpec.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\ColorResult.cs"; Dest = "Models\Cloud\Responses\ColorResult.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\Error.cs"; Dest = "Models\Cloud\Responses\Error.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\ErrorResponse.cs"; Dest = "Models\Cloud\Responses\ErrorResponse.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\Light.cs"; Dest = "Models\Cloud\Responses\Light.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\LightCollection.cs"; Dest = "Models\Cloud\Responses\LightCollection.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\Location.cs"; Dest = "Models\Cloud\Responses\Location.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\Result.cs"; Dest = "Models\Cloud\Responses\Result.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\Scene.cs"; Dest = "Models\Cloud\Responses\Scene.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    @{ Source = "Cloud\Models\Response\SuccessResponse.cs"; Dest = "Models\Cloud\Responses\SuccessResponse.cs"; OldNs = "Lifx.Api.Cloud.Models.Response"; NewNs = "Lifx.Api.Models.Cloud.Responses" }
    
    # Cloud Core Models
    @{ Source = "Cloud\Models\ColorHelpers.cs"; Dest = "Models\Cloud\ColorHelpers.cs"; OldNs = "Lifx.Api.Cloud.Models"; NewNs = "Lifx.Api.Models.Cloud" }
    @{ Source = "Cloud\Models\Group.cs"; Dest = "Models\Cloud\Group.cs"; OldNs = "Lifx.Api.Cloud.Models"; NewNs = "Lifx.Api.Models.Cloud" }
    @{ Source = "Cloud\Models\Hsbk.cs"; Dest = "Models\Cloud\Hsbk.cs"; OldNs = "Lifx.Api.Cloud.Models"; NewNs = "Lifx.Api.Models.Cloud" }
    @{ Source = "Cloud\Models\LifxColor.cs"; Dest = "Models\Cloud\LifxColor.cs"; OldNs = "Lifx.Api.Cloud.Models"; NewNs = "Lifx.Api.Models.Cloud" }
    @{ Source = "Cloud\Models\PowerState.cs"; Dest = "Models\Cloud\PowerState.cs"; OldNs = "Lifx.Api.Cloud.Models"; NewNs = "Lifx.Api.Models.Cloud" }
    @{ Source = "Cloud\Models\Selector.cs"; Dest = "Models\Cloud\Selector.cs"; OldNs = "Lifx.Api.Cloud.Models"; NewNs = "Lifx.Api.Models.Cloud" }
    
    # LAN Models
    @{ Source = "Lan\Color.cs"; Dest = "Models\Lan\Color.cs"; OldNs = "Lifx.Api.Lan"; NewNs = "Lifx.Api.Models.Lan" }
    @{ Source = "Lan\LightBulb.cs"; Dest = "Models\Lan\LightBulb.cs"; OldNs = "Lifx.Api.Lan"; NewNs = "Lifx.Api.Models.Lan" }
    @{ Source = "Lan\MessageType.cs"; Dest = "Models\Lan\MessageType.cs"; OldNs = "Lifx.Api.Lan"; NewNs = "Lifx.Api.Models.Lan" }
)

# Move files and update namespaces
Write-Host "Moving model files..." -ForegroundColor Yellow
foreach ($move in $fileMoves) {
    $sourcePath = Join-Path $projectRoot $move.Source
    $destPath = Join-Path $projectRoot $move.Dest
    
    if (Test-Path $sourcePath) {
        Write-Host "  Moving $($move.Source) -> $($move.Dest)" -ForegroundColor Cyan
        
        # Read content
        $content = Get-Content $sourcePath -Raw
        
        # Update namespace
        $content = $content -replace "namespace $([regex]::Escape($move.OldNs));", "namespace $($move.NewNs);"
        
        # Write to new location
        $destDir = Split-Path $destPath -Parent
        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Force -Path $destDir | Out-Null
        }
        Set-Content -Path $destPath -Value $content -NoNewline
        
        # Remove old file
        Remove-Item $sourcePath -Force
    } else {
        Write-Host "  Warning: Source file not found: $sourcePath" -ForegroundColor Red
    }
}

# Extract Device class from Discovery.cs and move it
Write-Host "Extracting Device class..." -ForegroundColor Yellow
$discoveryPath = Join-Path $projectRoot "Lan\LifxClient.Discovery.cs"
if (Test-Path $discoveryPath) {
    $discoveryContent = Get-Content $discoveryPath -Raw
    
    # Extract Device class
    if ($discoveryContent -match '(?s)(/// <summary>\r?\n/// LIFX Generic Device.*?^})') {
        $deviceClass = $matches[1]
        
        # Create Device.cs
        $deviceContent = @"
using Microsoft.Extensions.Logging;
using System.Net;

namespace Lifx.Api.Models.Lan;

$deviceClass
"@
        $devicePath = Join-Path $projectRoot "Models\Lan\Device.cs"
        Set-Content -Path $devicePath -Value $deviceContent
        Write-Host "  Created Device.cs" -ForegroundColor Cyan
        
        # Remove Device class from Discovery.cs but keep the rest
        $discoveryContent = $discoveryContent -replace '(?s)\r?\n\r?\n/// <summary>\r?\n/// LIFX Generic Device.*?^}', ''
        Set-Content -Path $discoveryPath -Value $discoveryContent -NoNewline
    }
}

# Update all using statements in remaining files
Write-Host "Updating using statements in all files..." -ForegroundColor Yellow

$namespaceReplacements = @{
    "using Lifx.Api.Cloud.Models.Request;" = "using Lifx.Api.Models.Cloud.Requests;"
    "using Lifx.Api.Cloud.Models.Response;" = "using Lifx.Api.Models.Cloud.Responses;"
    "using Lifx.Api.Cloud.Models;" = "using Lifx.Api.Models.Cloud;"
}

# Get all .cs files in the project (excluding obj/bin)
$csFiles = Get-ChildItem -Path $projectRoot -Filter "*.cs" -Recurse | 
    Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    foreach ($oldNs in $namespaceReplacements.Keys) {
        if ($content -match [regex]::Escape($oldNs)) {
            $content = $content -replace [regex]::Escape($oldNs), $namespaceReplacements[$oldNs]
            $modified = $true
        }
    }
    
    # Special handling for Selector namespace (keep it but update references)
    if ($content -match "using static Lifx\.Api\.Cloud\.Models\.Selector;") {
        $content = $content -replace "using static Lifx\.Api\.Cloud\.Models\.Selector;", "using static Lifx.Api.Models.Cloud.Selector;"
        $modified = $true
    }
    
    if ($modified) {
        Write-Host "  Updated: $($file.Name)" -ForegroundColor Gray
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}

# Clean up old directories if empty
Write-Host "Cleaning up old directories..." -ForegroundColor Yellow
$oldDirs = @(
    "Cloud\Models\Request",
    "Cloud\Models\Response",
    "Cloud\Models",
    "Cloud"
)

foreach ($dir in $oldDirs) {
    $dirPath = Join-Path $projectRoot $dir
    if ((Test-Path $dirPath) -and ((Get-ChildItem $dirPath).Count -eq 0)) {
        Remove-Item $dirPath -Force
        Write-Host "  Removed empty directory: $dir" -ForegroundColor Gray
    }
}

Write-Host "`nModel reorganization complete!" -ForegroundColor Green
Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "  - Moved $($fileMoves.Count) model files" -ForegroundColor White
Write-Host "  - Created Models/Cloud/Requests directory" -ForegroundColor White
Write-Host "  - Created Models/Cloud/Responses directory" -ForegroundColor White
Write-Host "  - Created Models/Lan directory" -ForegroundColor White
Write-Host "  - Updated all namespace references" -ForegroundColor White
Write-Host "`nPlease rebuild the solution to verify all changes." -ForegroundColor Cyan
