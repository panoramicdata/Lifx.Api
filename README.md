# Lifx.Api

## About

A unified API for LIFX supporting both Cloud HTTP API and LAN protocol. This library uses [Refit](https://github.com/reactiveui/refit) for the Cloud HTTP API client, providing a clean, type-safe, structured interface organized into logical sub-clients.

[![Nuget](https://img.shields.io/nuget/v/Lifx.Api)](https://www.nuget.org/packages/Lifx.Api/)

## Credits

Draws heavily from and combines previous work:
* [LifxCloudClient](https://github.com/isaacrlevin/LifxCloudClient) by [Isaac Levin](https://github.com/isaacrlevin)
* [LifxNet](https://github.com/dotMorten/LifxNet) by [Morten Nielsen](https://github.com/dotMorten)
* [LIFX's Cloud API documentation](https://api.developer.lifx.com/)
* [LIFX's LAN API documentation](https://lan.developer.lifx.com/docs)

## Usage

### LifxClient

The `LifxClient` is a unified client that supports both Cloud HTTP API and LAN protocol. You can enable one or both protocols based on your needs.

```csharp
using Lifx.Api;
using Lifx.Api.Cloud.Models;
using Lifx.Api.Cloud.Models.Request;
using Microsoft.Extensions.Logging;

// Create client with Cloud API only
var client = new LifxClient(new LifxClientOptions
{
    ApiToken = "your-api-token-here", // Get from https://cloud.lifx.com/settings
    Logger = loggerInstance, // Optional: for diagnostic logging
    IsLanEnabled = false // Optional: enable LAN protocol
});

// Or enable both Cloud and LAN
var client = new LifxClient(new LifxClientOptions
{
    ApiToken = "your-api-token-here",
    Logger = loggerInstance,
    IsLanEnabled = true // Enable LAN protocol for local network access
});

// Start LAN client if enabled
if (client.Lan is not null)
{
    client.StartLan(cancellationToken);
    client.StartDeviceDiscovery();
}
```

### Cloud API Operations

The Cloud API is organized into logical sub-clients with consistent async naming:

- **`client.Lights`** - Light operations (list, set state, toggle, etc.)
- **`client.Effects`** - Effect operations (breathe, pulse, clouds, sunrise, etc.)
- **`client.Scenes`** - Scene operations (list, activate)
- **`client.Color`** - Color operations (validate)

All async methods:
- End with `Async` suffix
- Require a `CancellationToken` parameter
- All parameters are mandatory (no optional parameters)

```csharp
// Lights operations
var lights = await client.Lights.ListAsync(Selector.All, cancellationToken);
var groups = await client.Lights.ListGroupsAsync(Selector.All, cancellationToken);
var locations = await client.Lights.ListLocationsAsync(Selector.All, cancellationToken);

await client.Lights.SetStateAsync(Selector.All, new SetStateRequest
{
    Power = PowerState.On,
    Color = "blue",
    Brightness = 0.5,
    Duration = 1.0
}, cancellationToken);

await client.Lights.TogglePowerAsync(Selector.All, new TogglePowerRequest
{
    Duration = 1.0
}, cancellationToken);

// Effects operations
await client.Effects.BreatheAsync(Selector.All, new BreatheEffectRequest
{
    Color = "red",
    FromColor = "blue",
    Period = 2.0,
    Cycles = 5
}, cancellationToken);

await client.Effects.PulseAsync(Selector.All, new PulseEffectRequest
{
    Color = "green",
    Period = 1.0,
    Cycles = 3
}, cancellationToken);

await client.Effects.SunriseAsync(Selector.All, new SunriseEffectRequest
{
    Duration = 60
}, cancellationToken);

await client.Effects.OffAsync(Selector.All, new EffectsOffRequest(), cancellationToken);

// Scenes operations
var scenes = await client.Scenes.ListAsync(cancellationToken);
await client.Scenes.ActivateAsync(scenes.First().UUID, new ActivateSceneRequest
{
    Duration = 2.0
}, cancellationToken);

// Color operations
var colorResult = await client.Color.ValidateColorAsync("rgb:255,0,0", cancellationToken);
Console.WriteLine($"Hue: {colorResult.Hue}, Saturation: {colorResult.Saturation}");
```

### JSON Serialization

The library provides centralized JSON serialization options that are used throughout:

```csharp
using System.Text.Json;

// Use the same serialization options as the LIFX API
var json = JsonSerializer.Serialize(myObject, LifxClient.JsonSerializerOptions);
var obj = JsonSerializer.Deserialize<MyType>(json, LifxClient.JsonSerializerOptions);
```

The `LifxClient.JsonSerializerOptions` uses:
- **snake_case_lower** for property names
- **Enum member values** (e.g., "on"/"off" for PowerState)
- **Null value ignoring** when serializing

### LAN Protocol Operations

When LAN is enabled, you can access the LAN client through `client.Lan`:

```csharp
var client = new LifxClient(new LifxClientOptions
{
    ApiToken = null, // Optional: can use LAN without Cloud API
    Logger = loggerInstance,
    IsLanEnabled = true
});

// Start LAN client
client.StartLan(cancellationToken);

// Discover devices
client.Lan.DeviceDiscovered += (sender, e) =>
{
    Console.WriteLine($"Found: {e.Device.MacAddressName}");
};

client.StartDeviceDiscovery();

// Access discovered devices
foreach (var device in client.Lan.Devices)
{
    if (device is LightBulb bulb)
    {
        await client.Lan.TurnBulbOnAsync(bulb, TimeSpan.FromSeconds(1));
    }
}
```

## Features

### Cloud API Features
- ? **Lights** - List lights, groups, and locations; Set state (power, color, brightness); Toggle power; Cycle states; HEV clean
- ? **Effects** - Breathe, Pulse, Morph, Flame, Move, Clouds, Sunrise, Sunset
- ? **Scenes** - List and activate scenes
- ? **Color** - Validate color strings
- ? Built with Refit for type-safe HTTP calls
- ? Structured sub-client API (`client.Lights.*`, `client.Effects.*`, etc.)
- ? All async methods follow `Async` suffix convention
- ? All methods require `CancellationToken` for proper cancellation support
- ? Centralized JSON serialization options
- ? Comprehensive logging support

### LAN Protocol Features
- ? Direct local network communication
- ? Device discovery
- ? Power control with transitions
- ? Color and temperature control
- ? Infrared support
- ? Can be used independently or alongside Cloud API

## API Conventions

All async methods follow these conventions:
1. ? Method names end with `Async` suffix
2. ? All parameters are mandatory (no optional parameters)
3. ? All methods accept `CancellationToken` as the last parameter
4. ? Consistent naming across all sub-clients
5. ? Consistent JSON serialization via `LifxClient.JsonSerializerOptions`

## Requirements

- .NET 10.0 or later
- For Cloud API: LIFX API token from https://cloud.lifx.com/settings
- For LAN: Devices on the same local network
