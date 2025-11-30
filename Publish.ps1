#Requires -Version 7.0

<#
.SYNOPSIS
    Automated NuGet package publishing script for Lifx.Api

.DESCRIPTION
    This script automates the entire publishing process:
    - Checks for clean git working directory (porcelain)
    - Runs all tests
    - Gets version from Nerdbank.GitVersioning
    - Creates git tag
    - Builds NuGet package
    - Publishes to NuGet.org

.NOTES
    Requires:
    - PowerShell 7.0+
    - .NET 10.0 SDK
    - nbgv tool (dotnet tool install -g nbgv)
    - nuget-key.txt file in the repository root

.EXAMPLE
    .\Publish.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

#region Helper Functions

function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "    ERROR: $Message" -ForegroundColor Red
}

function Test-Command {
    param([string]$Command)
    $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

function Exit-WithError {
    param([string]$Message)
    Write-Error $Message
    exit 1
}

#endregion

#region Prerequisites Check

Write-Step "Checking prerequisites..."

# Check PowerShell version
if ($PSVersionTable.PSVersion.Major -lt 7) {
    Exit-WithError "PowerShell 7.0 or later is required. Current version: $($PSVersionTable.PSVersion)"
}
Write-Success "PowerShell version: $($PSVersionTable.PSVersion)"

# Check for .NET SDK
if (-not (Test-Command "dotnet")) {
    Exit-WithError ".NET SDK not found. Please install .NET 10.0 SDK or later."
}
$dotnetVersion = dotnet --version
Write-Success ".NET SDK version: $dotnetVersion"

# Check for git
if (-not (Test-Command "git")) {
    Exit-WithError "Git not found. Please install Git."
}
Write-Success "Git found"

# Check for nbgv
if (-not (Test-Command "nbgv")) {
    Exit-WithError "nbgv not found. Install with: dotnet tool install -g nbgv"
}
Write-Success "nbgv found"

# Check for nuget-key.txt
$nugetKeyPath = Join-Path $PSScriptRoot "nuget-key.txt"
if (-not (Test-Path $nugetKeyPath)) {
    Exit-WithError "nuget-key.txt not found in repository root. Create this file with your NuGet API key."
}
Write-Success "nuget-key.txt found"

# Read NuGet API key
$nugetKey = (Get-Content $nugetKeyPath -Raw).Trim()
if ([string]::IsNullOrWhiteSpace($nugetKey)) {
    Exit-WithError "nuget-key.txt is empty. Add your NuGet API key to this file."
}
Write-Success "NuGet API key loaded"

#endregion

#region Git Status Check

Write-Step "Checking git status..."

# Check if we're in a git repository
try {
    $gitStatus = git status --porcelain 2>&1
    if ($LASTEXITCODE -ne 0) {
        Exit-WithError "Not in a git repository or git error occurred."
    }
}
catch {
    Exit-WithError "Failed to check git status: $_"
}

# Check for uncommitted changes
if ($gitStatus) {
    Write-Error "Git working directory is not clean. Uncommitted changes:"
    Write-Host $gitStatus -ForegroundColor Yellow
    Exit-WithError "Commit or stash changes before publishing."
}
Write-Success "Git working directory is clean (porcelain)"

# Get current branch
$currentBranch = git rev-parse --abbrev-ref HEAD
Write-Success "Current branch: $currentBranch"

# Check if we're on main/master
if ($currentBranch -ne "main" -and $currentBranch -ne "master") {
    Write-Host "    WARNING: Not on main/master branch. Current branch: $currentBranch" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (y/N)"
    if ($continue -ne "y" -and $continue -ne "Y") {
        Exit-WithError "Publish cancelled by user."
    }
}

#endregion

#region Get Version

Write-Step "Getting version from Nerdbank.GitVersioning..."

try {
    $versionJson = nbgv get-version --format json | ConvertFrom-Json
    $version = $versionJson.NuGetPackageVersion
    $simpleVersion = $versionJson.SimpleVersion
    
    if ([string]::IsNullOrWhiteSpace($version)) {
        Exit-WithError "Failed to get version from nbgv."
    }
    
    Write-Success "Version: $version"
    Write-Success "Simple Version: $simpleVersion"
}
catch {
    Exit-WithError "Failed to get version from nbgv: $_"
}

# Check if tag already exists
$tagExists = git tag -l "v$simpleVersion"
if ($tagExists) {
    Exit-WithError "Git tag 'v$simpleVersion' already exists. Version must be incremented."
}

#endregion

#region Run Tests

Write-Step "Running all tests..."

try {
    # Clean previous test results
    $testResultsDir = Join-Path $PSScriptRoot "TestResults"
    if (Test-Path $testResultsDir) {
        Remove-Item $testResultsDir -Recurse -Force
    }
    
    # Run tests
    $testOutput = dotnet test --configuration Release --verbosity normal 2>&1
    $testExitCode = $LASTEXITCODE
    
    # Display output
    $testOutput | ForEach-Object { Write-Host "    $_" }
    
    if ($testExitCode -ne 0) {
        Exit-WithError "Tests failed. Fix failing tests before publishing."
    }
    
    # Parse test results
    $passedTests = ($testOutput | Select-String -Pattern "Passed.*(\d+)" | ForEach-Object { $_.Matches.Groups[1].Value }) -join ""
    $totalTests = ($testOutput | Select-String -Pattern "Total.*(\d+)" | ForEach-Object { $_.Matches.Groups[1].Value }) -join ""
    
    if ($passedTests -and $totalTests) {
        Write-Success "All tests passed: $passedTests/$totalTests"
    }
    else {
        Write-Success "Tests completed successfully"
    }
}
catch {
    Exit-WithError "Failed to run tests: $_"
}

#endregion

#region Build Package

Write-Step "Building NuGet package..."

try {
    # Clean previous builds
    $packOutputDir = Join-Path $PSScriptRoot "nupkgs"
    if (Test-Path $packOutputDir) {
        Remove-Item $packOutputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $packOutputDir -Force | Out-Null
    
    # Build package
    $packOutput = dotnet pack ./Lifx.Api/Lifx.Api.csproj `
        --configuration Release `
        --output $packOutputDir `
        --verbosity normal `
        2>&1
    
    if ($LASTEXITCODE -ne 0) {
        $packOutput | ForEach-Object { Write-Host "    $_" }
        Exit-WithError "Failed to build NuGet package."
    }
    
    # Find the generated package
    $packageFile = Get-ChildItem -Path $packOutputDir -Filter "Lifx.Api.$version.nupkg" -ErrorAction SilentlyContinue
    
    if (-not $packageFile) {
        # Try without exact version match
        $packageFile = Get-ChildItem -Path $packOutputDir -Filter "Lifx.Api.*.nupkg" | Select-Object -First 1
    }
    
    if (-not $packageFile) {
        Exit-WithError "NuGet package not found in $packOutputDir"
    }
    
    Write-Success "Package built: $($packageFile.Name)"
    Write-Success "Package size: $([math]::Round($packageFile.Length / 1KB, 2)) KB"
}
catch {
    Exit-WithError "Failed to build package: $_"
}

#endregion

#region Create Git Tag

Write-Step "Creating git tag..."

try {
    $tagName = "v$simpleVersion"
    $tagMessage = "Release $version"
    
    git tag -a $tagName -m $tagMessage
    
    if ($LASTEXITCODE -ne 0) {
        Exit-WithError "Failed to create git tag."
    }
    
    Write-Success "Created tag: $tagName"
}
catch {
    Exit-WithError "Failed to create git tag: $_"
}

#endregion

#region Publish to NuGet

Write-Step "Publishing to NuGet.org..."

try {
    $publishOutput = dotnet nuget push $packageFile.FullName `
        --api-key $nugetKey `
        --source https://api.nuget.org/v3/index.json `
        --skip-duplicate `
        2>&1
    
    # Display output (but hide API key)
    $publishOutput | ForEach-Object { 
        $line = $_ -replace $nugetKey, "***HIDDEN***"
        Write-Host "    $line"
    }
    
    if ($LASTEXITCODE -ne 0) {
        # Rollback git tag
        Write-Host "    Rolling back git tag..." -ForegroundColor Yellow
        git tag -d $tagName
        Exit-WithError "Failed to publish to NuGet."
    }
    
    Write-Success "Package published successfully!"
}
catch {
    # Rollback git tag
    Write-Host "    Rolling back git tag..." -ForegroundColor Yellow
    git tag -d $tagName
    Exit-WithError "Failed to publish to NuGet: $_"
}

#endregion

#region Push Git Tag

Write-Step "Pushing git tag to remote..."

try {
    git push origin $tagName
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "    WARNING: Failed to push tag to remote. Tag created locally only." -ForegroundColor Yellow
    }
    else {
        Write-Success "Tag pushed to remote"
    }
}
catch {
    Write-Host "    WARNING: Failed to push tag to remote: $_" -ForegroundColor Yellow
}

#endregion

#region Summary

Write-Host "`n" -NoNewline
Write-Host "========================================" -ForegroundColor Green
Write-Host "  PUBLISH SUCCESSFUL!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Package:  Lifx.Api v$version" -ForegroundColor White
Write-Host "  Tag:      $tagName" -ForegroundColor White
Write-Host "  File:     $($packageFile.Name)" -ForegroundColor White
Write-Host ""
Write-Host "  NuGet:    https://www.nuget.org/packages/Lifx.Api/$version" -ForegroundColor Cyan
Write-Host ""
Write-Host "NOTE: It may take a few minutes for the package to appear on NuGet.org" -ForegroundColor Yellow
Write-Host ""

#endregion
