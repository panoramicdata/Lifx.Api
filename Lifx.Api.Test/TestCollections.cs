namespace Lifx.Api.Test;

/// <summary>
/// Collection definition for LIFX Cloud API tests.
/// These tests use the LIFX Cloud API and may make actual HTTP calls.
/// </summary>
[CollectionDefinition("Cloud API Tests", DisableParallelization = false)]
public class CloudApiTestCollection
{
}

/// <summary>
/// Collection definition for LAN Protocol tests.
/// These tests use the LIFX LAN protocol for local network communication.
/// </summary>
[CollectionDefinition("LAN Protocol Tests", DisableParallelization = false)]
public class LanProtocolTestCollection
{
}

/// <summary>
/// Collection definition for Model/Serialization tests.
/// These tests verify JSON serialization/deserialization and model behavior.
/// Tests in this collection are fast and don't require external dependencies.
/// </summary>
[CollectionDefinition("Model Tests", DisableParallelization = false)]
public class ModelTestCollection
{
}

/// <summary>
/// Collection definition for Unit tests.
/// These tests are pure unit tests with no external dependencies.
/// Tests in this collection should be very fast.
/// </summary>
[CollectionDefinition("Unit Tests", DisableParallelization = false)]
public class UnitTestCollection
{
}