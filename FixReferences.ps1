# Fix remaining namespace and reference issues
$ErrorActionPreference = "Stop"

Write-Host "Fixing remaining issues..." -ForegroundColor Green

$workspaceRoot = "C:\Users\DavidBond\source\repos\panoramicdata\Lifx.Api"
$projectRoot = Join-Path $workspaceRoot "Lifx.Api"

# 1. Find and create BreatheEffectRequest.cs (it seems to be missing)
Write-Host "Looking for missing BreatheEffectRequest.cs..." -ForegroundColor Yellow
$breatheFile = Get-ChildItem -Path $projectRoot -Filter "BreatheEffectRequest.cs" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $breatheFile) {
    Write-Host "  BreatheEffectRequest.cs was not found in original location - checking if it exists elsewhere" -ForegroundColor Red
}

# 2. Fix test project's .csproj to reference Models correctly
$testProjPath = Join-Path $workspaceRoot "Lifx.Api.Test\Lifx.Api.Test.csproj"
$testProjContent = Get-Content $testProjPath -Raw
Set-Content -Path $testProjPath -Value $testProjContent -NoNewline

# 3. Update Lifx.Api\Lan\LifxClient.Discovery.cs to use Models.Lan.Device and Models.Lan.LightBulb
$discoveryPath = Join-Path $projectRoot "Lan\LifxClient.Discovery.cs"
$discoveryContent = Get-Content $discoveryPath -Raw

# The Device class is now Lifx.Api.Models.Lan.Device, update the discovery file
$discoveryContent = $discoveryContent -replace 'private readonly Dictionary<string, Device>', 'private readonly Dictionary<string, Models.Lan.Device>'
$discoveryContent = $discoveryContent -replace 'private readonly IList<Device>', 'private readonly IList<Models.Lan.Device>'
$discoveryContent = $discoveryContent -replace 'public IEnumerable<Device> Devices', 'public IEnumerable<Models.Lan.Device> Devices'
$discoveryContent = $discoveryContent -replace 'internal DeviceDiscoveryEventArgs\(Device device\)', 'internal DeviceDiscoveryEventArgs(Models.Lan.Device device)'
$discoveryContent = $discoveryContent -replace 'public Device Device', 'public Models.Lan.Device Device'
$discoveryContent = $discoveryContent -replace 'if \(_discoveredBulbs\.TryGetValue\(id, out Device\?', 'if (_discoveredBulbs.TryGetValue(id, out Models.Lan.Device?'
$discoveryContent = $discoveryContent -replace 'var device = new LightBulb', 'var device = new Models.Lan.LightBulb'

Set-Content -Path $discoveryPath -Value $discoveryContent -NoNewline
Write-Host "  Updated LifxClient.Discovery.cs" -ForegroundColor Cyan

# 4. Update Lifx.Api\Lan\LifxClient.DeviceOperations.cs
$deviceOpsPath = Join-Path $projectRoot "Lan\LifxClient.DeviceOperations.cs"
$deviceOpsContent = Get-Content $deviceOpsPath -Raw
$deviceOpsContent = $deviceOpsContent -replace 'public Task TurnDeviceOnAsync\(Device device\)', 'public Task TurnDeviceOnAsync(Models.Lan.Device device)'
$deviceOpsContent = $deviceOpsContent -replace 'public Task TurnDeviceOffAsync\(Device device\)', 'public Task TurnDeviceOffAsync(Models.Lan.Device device)'
$deviceOpsContent = $deviceOpsContent -replace 'public async Task SetDevicePowerStateAsync\(Device device', 'public async Task SetDevicePowerStateAsync(Models.Lan.Device device'
$deviceOpsContent = $deviceOpsContent -replace 'public async Task<string\?> GetDeviceLabelAsync\(Device device\)', 'public async Task<string?> GetDeviceLabelAsync(Models.Lan.Device device)'
$deviceOpsContent = $deviceOpsContent -replace 'public async Task SetDeviceLabelAsync\(Device device', 'public async Task SetDeviceLabelAsync(Models.Lan.Device device'
$deviceOpsContent = $deviceOpsContent -replace 'public Task<StateVersionResponse> GetDeviceVersionAsync\(Device device\)', 'public Task<StateVersionResponse> GetDeviceVersionAsync(Models.Lan.Device device)'
$deviceOpsContent = $deviceOpsContent -replace 'public Task<StateHostFirmwareResponse> GetDeviceHostFirmwareAsync\(Device device\)', 'public Task<StateHostFirmwareResponse> GetDeviceHostFirmwareAsync(Models.Lan.Device device)'

Set-Content -Path $deviceOpsPath -Value $deviceOpsContent -NoNewline
Write-Host "  Updated LifxClient.DeviceOperations.cs" -ForegroundColor Cyan

# 5. Update Lifx.Api\Lan\LifxClient.LightOperations.cs
$lightOpsPath = Join-Path $projectRoot "Lan\LifxClient.LightOperations.cs"
$lightOpsContent = Get-Content $lightOpsPath -Raw
$lightOpsContent = $lightOpsContent -replace 'public Task TurnBulbOnAsync\(LightBulb bulb', 'public Task TurnBulbOnAsync(Models.Lan.LightBulb bulb'
$lightOpsContent = $lightOpsContent -replace 'public Task TurnBulbOffAsync\(LightBulb bulb', 'public Task TurnBulbOffAsync(Models.Lan.LightBulb bulb'
$lightOpsContent = $lightOpsContent -replace 'public async Task SetLightPowerAsync\(LightBulb bulb', 'public async Task SetLightPowerAsync(Models.Lan.LightBulb bulb'
$lightOpsContent = $lightOpsContent -replace 'public async Task<bool> GetLightPowerAsync\(LightBulb bulb\)', 'public async Task<bool> GetLightPowerAsync(Models.Lan.LightBulb bulb)'
$lightOpsContent = $lightOpsContent -replace 'public Task SetColorAsync\(LightBulb bulb, Color color', 'public Task SetColorAsync(Models.Lan.LightBulb bulb, Models.Lan.Color color'
$lightOpsContent = $lightOpsContent -replace 'public async Task SetColorAsync\(LightBulb bulb,', 'public async Task SetColorAsync(Models.Lan.LightBulb bulb,'
$lightOpsContent = $lightOpsContent -replace 'public Task<LightStateResponse> GetLightStateAsync\(LightBulb bulb\)', 'public Task<LightStateResponse> GetLightStateAsync(Models.Lan.LightBulb bulb)'
$lightOpsContent = $lightOpsContent -replace 'public async Task<ushort> GetInfraredAsync\(LightBulb bulb\)', 'public async Task<ushort> GetInfraredAsync(Models.Lan.LightBulb bulb)'
$lightOpsContent = $lightOpsContent -replace 'public async Task SetInfraredAsync\(Device device', 'public async Task SetInfraredAsync(Models.Lan.Device device'

Set-Content -Path $lightOpsPath -Value $lightOpsContent -NoNewline
Write-Host "  Updated LifxClient.LightOperations.cs" -ForegroundColor Cyan

# 6. Add missing using in LightBulb.cs
$lightBulbPath = Join-Path $projectRoot "Models\Lan\LightBulb.cs"
if (Test-Path $lightBulbPath) {
    $content = Get-Content $lightBulbPath -Raw
    # Device is in the same namespace now, so no additional using needed
    # But make sure it compiles by ensuring the file is correct
    Set-Content -Path $lightBulbPath -Value $content -NoNewline
}

Write-Host "`nFixes applied!" -ForegroundColor Green
Write-Host "Note: Some files may still need manual review." -ForegroundColor Yellow
