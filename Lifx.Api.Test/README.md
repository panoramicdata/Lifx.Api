# Lifx.Api.Test

xUnit v3 test project for Lifx.Api using AwesomeAssertions.

## Setup

You can configure the LIFX Cloud API token using either User Secrets (recommended) or appsettings.json:

### Option 1: User Secrets (Recommended)
```bash
dotnet user-secrets set "AppToken" "your-lifx-cloud-api-token-here" --project Lifx.Api.Test
```

### Option 2: appsettings.json
1. Copy `appsettings.example.json` to `appsettings.json`
2. Edit `appsettings.json` and add your LIFX Cloud API token
   - Get your token from https://cloud.lifx.com/settings
3. Run the tests

## Test Structure

- **SelectorTests.cs** - Unit tests for Selector class
- **PowerStateTests.cs** - Unit tests for PowerState enum serialization
- **LifxColorTests.cs** - Unit tests for LifxColor helper methods
- **LightsTests.cs** - Integration tests for the LIFX Cloud API

## Notes

- User Secrets keeps sensitive data out of source control
- Integration tests require a valid LIFX Cloud API token
- Integration tests require active LIFX devices on your account
- Tests are organized into the "Cloud API Tests" collection
