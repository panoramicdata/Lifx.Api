# Lifx.Api

A unified .NET API for LIFX smart lighting supporting both Cloud HTTP API and LAN protocol.

[![Nuget](https://img.shields.io/nuget/v/Lifx.Api)](https://www.nuget.org/packages/Lifx.Api/)
[![Nuget](https://img.shields.io/nuget/dt/Lifx.Api)](https://www.nuget.org/packages/Lifx.Api/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/270e155e044443ca9dad25c37ffa8208)](https://app.codacy.com/gh/panoramicdata/Lifx.Api/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

## Description

Lifx.Api is a comprehensive .NET library that provides a unified interface for controlling LIFX smart lights through both their Cloud HTTP API and LAN protocol. Built with [Refit](https://github.com/reactiveui/refit) for type-safe HTTP calls, it offers a clean, modern API organized into logical sub-clients.

## Features

### Cloud API Features
- **Lights** - List lights, groups, and locations; Set state (power, color, brightness); Toggle power; Cycle states; HEV clean
- **Effects** - Breathe, Pulse, Morph, Flame, Move, Clouds, Sunrise, Sunset
- **Scenes** - List and activate scenes
- **Color** - Validate color strings and conversions
- Built with Refit for type-safe HTTP calls
- Structured sub-client API (`client.Lights.*`, `client.Effects.*`, etc.)
- All async methods follow `Async` suffix convention
- All methods require `CancellationToken` for proper cancellation support
- Centralized JSON serialization with snake_case naming
- Comprehensive logging support

### LAN Protocol Features
- Direct local network communication (no internet required)
- Automatic device discovery via UDP broadcast
- Power control with smooth transitions
- Color and temperature control
- Infrared support for compatible bulbs
- Can be used independently or alongside Cloud API
- Lower latency than Cloud API

### General Features
- Models are POCOs (no dependencies, easy to serialize)
- Fully async/await throughout
- Comprehensive error handling
- Extensive unit and integration test coverage (232+ tests, 92% coverage)
- .NET 10.0 support
- MIT licensed

## Installation

### Library

```bash
dotnet add package Lifx.Api
```

Or via NuGet Package Manager:

```powershell
Install-Package Lifx.Api
```

### CLI Tool

Install the LIFX CLI tool globally:

```bash
dotnet tool install --global Lifx.Cli
```

Once installed, you can use the `lifx` command from anywhere:

```bash
lifx --help
```

To update to the latest version:

```bash
dotnet tool update --global Lifx.Cli
```

To uninstall:

```bash
dotnet tool uninstall --global Lifx.Cli
```

## Quick Start

### Cloud API Only

```csharp
using Lifx.Api;

var client = new LifxClient(new LifxClientOptions
{
    ApiToken = "your-api-token-here" // Get from https://cloud.lifx.com/settings
});

// List all lights
var lights = await client.Lights.ListAsync(Selector.All, cancellationToken);

// Turn all lights on with blue color
await client.Lights.SetStateAsync(Selector.All, new SetStateRequest
{
    Power = PowerState.On,
    Color = "blue",
    Brightness = 0.8,
    Duration = 1.0
}, cancellationToken);
```

### LAN Protocol Only

```csharp
using Lifx.Api;

var client = new LifxClient(new LifxClientOptions
{
    IsLanEnabled = true
});

client.StartLan(cancellationToken);

// Discover devices
client.Lan.DeviceDiscovered += (sender, e) =>
{
    Console.WriteLine($"Found: {e.Device.MacAddressName}");
};

client.StartDeviceDiscovery(cancellationToken);

// Control discovered devices
foreach (var device in client.Lan.Devices)
{
    if (device is LightBulb bulb)
    {
        await client.Lan.TurnBulbOnAsync(bulb, TimeSpan.FromSeconds(1), cancellationToken);
    }
}
```

### Both Cloud and LAN

```csharp
using Lifx.Api;

var client = new LifxClient(new LifxClientOptions
{
    ApiToken = "your-api-token-here",
    IsLanEnabled = true,
    Logger = loggerInstance // Optional: for diagnostics
});

client.StartLan(cancellationToken);
client.StartDeviceDiscovery(cancellationToken);

// Use Cloud API for queries
var lights = await client.Lights.ListAsync(Selector.All, cancellationToken);

// Use LAN for low-latency control
var localBulb = client.Lan.Devices.OfType<LightBulb>().FirstOrDefault();
if (localBulb != null)
{
    await client.Lan.SetColorAsync(
        localBulb, 
        new Color { R = 255, G = 0, B = 0 }, 
        kelvin: 3500, 
        cancellationToken);
}
```

## Usage Examples

### Lights Operations

```csharp
// List all lights
var lights = await client.Lights.ListAsync(Selector.All, cancellationToken);

// List lights in a specific group
var groupLights = await client.Lights.ListAsync(
    new Selector.GroupLabel("Living Room"), 
    cancellationToken);

// Get groups and locations
var groups = await client.Lights.ListGroupsAsync(Selector.All, cancellationToken);
var locations = await client.Lights.ListLocationsAsync(Selector.All, cancellationToken);

// Set state for a specific light
await client.Lights.SetStateAsync(
    new Selector.LightId("d073d5000001"), 
    new SetStateRequest
    {
        Power = PowerState.On,
        Color = "rgb:255,128,0", // Orange
        Brightness = 0.7,
        Duration = 2.0
    }, 
    cancellationToken);

// Toggle power
await client.Lights.TogglePowerAsync(Selector.All, new TogglePowerRequest
{
    Duration = 1.0
}, cancellationToken);

// Set multiple lights with different colors
await client.Lights.SetStatesAsync(new SetStatesRequest
{
    States = new[]
    {
        new StateUpdate { Selector = "id:light1", Color = "red" },
        new StateUpdate { Selector = "id:light2", Color = "blue" }
    },
    Defaults = new StateDefaults { Duration = 1.0 }
}, cancellationToken);

// Adjust brightness incrementally
await client.Lights.StateDeltaAsync(
    Selector.All, 
    new StateDeltaRequest { Brightness = 0.1 }, // Increase by 10%
    cancellationToken);
```

### Effects

```csharp
// Breathe effect
await client.Effects.BreatheAsync(Selector.All, new BreatheEffectRequest
{
    Color = "red",
    FromColor = "blue",
    Period = 2.0,
    Cycles = 5.0,
    Persist = false,
    PowerOn = true
}, cancellationToken);

// Pulse effect
await client.Effects.PulseAsync(Selector.All, new PulseEffectRequest
{
    Color = "green",
    Period = 1.0,
    Cycles = 3.0
}, cancellationToken);

// Morph effect (smooth color transitions)
await client.Effects.MorphAsync(Selector.All, new MorphEffectRequest
{
    Period = 3.0,
    Duration = 30.0,
    PowerOn = true
}, cancellationToken);

// Flame effect
await client.Effects.FlameAsync(Selector.All, new FlameEffectRequest
{
    Period = 5.0,
    Duration = 60.0
}, cancellationToken);

// Multi-zone effects
await client.Effects.MoveAsync(
    new Selector.GroupLabel("Kitchen"), 
    new MoveEffectRequest
    {
        Direction = "forward",
        Period = 2.0
    }, 
    cancellationToken);

await client.Effects.CloudsAsync(
    new Selector.GroupLabel("Bedroom"), 
    new CloudsEffectRequest
    {
        Duration = 120
    }, 
    cancellationToken);

// Time-based effects
await client.Effects.SunriseAsync(Selector.All, new SunriseEffectRequest
{
    Duration = 300 // 5 minutes
}, cancellationToken);

await client.Effects.SunsetAsync(Selector.All, new SunsetEffectRequest
{
    Duration = 300
}, cancellationToken);

// Stop all effects
await client.Effects.OffAsync(Selector.All, new EffectsOffRequest
{
    PowerOff = false
}, cancellationToken);
```

### Scenes

```csharp
// List all scenes
var scenes = await client.Scenes.ListScenesAsync(cancellationToken);

foreach (var scene in scenes)
{
    Console.WriteLine($"{scene.Name} (UUID: {scene.Uuid})");
}

// Activate a scene
await client.Scenes.ActivateSceneAsync(
    $"scene_id:{scenes.First().Uuid}", 
    new ActivateSceneRequest
    {
        Duration = 2.0,
        Fast = false
    }, 
    cancellationToken);
```

### Color Validation

```csharp
// Validate color strings
var colorResult = await client.Color.ValidateColorAsync("rgb:255,0,0", cancellationToken);
Console.WriteLine($"Hue: {colorResult.Hue}, Saturation: {colorResult.Saturation}");

// Use LifxColor helper for building color strings
var rgbColor = LifxColor.BuildRGB(255, 128, 0); // "rgb:255,128,0"
var hsbkColor = LifxColor.BuildHSBK(120, 1.0, 0.8, 3500); // "hue:120 saturation:1 brightness:0.8 kelvin:3500"

// Named colors
var namedColor = LifxColor.NamedColors; // ["white", "red", "orange", "yellow", "green", "cyan", "blue", "purple", "pink"]
```

### Selectors

```csharp
// All lights
Selector.All

// Specific light by ID
new Selector.LightId("d073d5000001")

// Light by label
new Selector.LightLabel("Bedroom Lamp")

// Group by ID or label
new Selector.GroupId("group123")
new Selector.GroupLabel("Living Room")

// Location by ID or label
new Selector.LocationId("location456")
new Selector.LocationLabel("Home")

// Implicit conversion from Light or LightCollection
Light light = await GetLightAsync();
await client.Lights.SetStateAsync(light, request, cancellationToken); // Implicit conversion to Selector
```

### LAN Protocol

```csharp
// Start LAN client
client.StartLan(cancellationToken);

// Device discovery with event handler
client.Lan.DeviceDiscovered += (sender, e) =>
{
    Console.WriteLine($"Found device: {e.Device.MacAddressName} at {e.Device.HostName}");
};

client.StartDeviceDiscovery(cancellationToken);

// Access discovered devices
var devices = client.Lan.Devices;

foreach (var device in devices)
{
    if (device is LightBulb bulb)
    {
        // Turn on
        await client.Lan.TurnBulbOnAsync(bulb, TimeSpan.FromSeconds(1), cancellationToken);
        
        // Set color (RGB)
        await client.Lan.SetColorAsync(
            bulb, 
            new Color { R = 255, G = 0, B = 0 }, 
            kelvin: 3500, 
            cancellationToken);
        
        // Set color (HSBK)
        await client.Lan.SetColorAsync(
            bulb,
            hue: 120,
            saturation: 65535,
            brightness: 32768,
            kelvin: 3500,
            transitionDuration: TimeSpan.FromMilliseconds(500),
            cancellationToken);
        
        // Get state
        var state = await client.Lan.GetLightStateAsync(bulb, cancellationToken);
        
        // Set infrared (for supported bulbs)
        await client.Lan.SetInfraredAsync(bulb, brightness: 32768, cancellationToken);
    }
}

// Stop discovery
client.StopDeviceDiscovery();
```

### JSON Serialization

The library uses System.Text.Json with custom options for compatibility with the LIFX API:

```csharp
using System.Text.Json;

// Use the same serialization options as the library
var json = JsonSerializer.Serialize(myObject, LifxClient.JsonSerializerOptions);
var obj = JsonSerializer.Deserialize<MyType>(json, LifxClient.JsonSerializerOptions);

// The options use:
// - snake_case property names
// - Enum string values (e.g., "on"/"off" for PowerState)
// - Null value ignoring
```

### Error Handling

```csharp
try
{
    await client.Lights.SetStateAsync(Selector.All, request, cancellationToken);
}
catch (ApiException ex)
{
    // Refit API exception (HTTP errors)
    Console.WriteLine($"API Error: {ex.StatusCode} - {ex.Content}");
}
catch (ArgumentException ex)
{
    // Validation errors (invalid parameters)
    Console.WriteLine($"Validation Error: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    // State errors (e.g., LAN not enabled)
    Console.WriteLine($"Operation Error: {ex.Message}");
}
```

## API Conventions

All async methods follow these conventions:

1. Method names end with `Async` suffix
2. All parameters are mandatory (no optional parameters)
3. All methods accept `CancellationToken` as the last parameter
4. Consistent naming across all sub-clients
5. Models are POCOs with no dependencies

## Requirements

- .NET 10.0 or later
- For Cloud API: LIFX API token from [https://cloud.lifx.com/settings](https://cloud.lifx.com/settings)
- For LAN: LIFX devices on the same local network

## Testing

The library has comprehensive test coverage:

- 232+ tests with 92% code coverage
- Unit tests for utilities, models, and validation
- Integration tests for Cloud API operations
- LAN protocol tests with proper fixture management
- Tests organized by namespace (Cloud, LAN, Unit)

Run tests:

```bash
dotnet test
```

Run specific test categories:

```bash
# Cloud API tests only
dotnet test --filter "FullyQualifiedName~Lifx.Api.Test.Cloud"

# LAN tests only
dotnet test --filter "FullyQualifiedName~Lifx.Api.Test.Lan"

# Unit tests only
dotnet test --filter "FullyQualifiedName~Lifx.Api.Test.Unit"
```

## Architecture

### Models as POCOs

All models are Plain Old CLR Objects with no dependencies:

- No client references
- No action methods
- Easy to serialize/deserialize
- Easy to test
- Follows SOLID principles

### Sub-Client Organization

The API is organized into logical sub-clients:

- `client.Lights` - Light operations
- `client.Effects` - Effect operations
- `client.Scenes` - Scene operations
- `client.Color` - Color operations
- `client.Lan` - LAN protocol operations

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Publishing

For maintainers publishing new versions to NuGet:

1. Ensure you have a NuGet API key from https://www.nuget.org/account/apikeys
2. Create `nuget-key.txt` in the repository root with your API key
3. Run the automated publish script:

```powershell
.\Publish.ps1
```

The script will:
- Verify clean git state (porcelain)
- Run all tests
- Get version from Nerdbank.GitVersioning
- Build and publish NuGet package
- Create and push git tag

See [PUBLISHING.md](PUBLISHING.md) for detailed publishing instructions and [PRE-PUBLISH-CHECKLIST.md](PRE-PUBLISH-CHECKLIST.md) for pre-flight checks.

## Credits

This library draws heavily from and combines previous work:

- [LifxCloudClient](https://github.com/isaacrlevin/LifxCloudClient) by [Isaac Levin](https://github.com/isaacrlevin)
- [LifxNet](https://github.com/dotMorten/LifxNet) by [Morten Nielsen](https://github.com/dotMorten)
- [LIFX's Cloud API documentation](https://api.developer.lifx.com/)
- [LIFX's LAN API documentation](https://lan.developer.lifx.com/docs)

## License

MIT - See [LICENSE](LICENSE) for details.

## Links

- [NuGet Package](https://www.nuget.org/packages/Lifx.Api/)
- [GitHub Repository](https://github.com/panoramicdata/Lifx.Api)
- [LIFX Cloud API Documentation](https://api.developer.lifx.com/)
- [LIFX LAN Protocol Documentation](https://lan.developer.lifx.com/docs)
- [Issue Tracker](https://github.com/panoramicdata/Lifx.Api/issues)
