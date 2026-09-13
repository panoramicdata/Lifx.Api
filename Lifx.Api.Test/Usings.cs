global using Xunit;

// The run-wide safety net for real light state; see CloudLightStateGuard.
[assembly: AssemblyFixture(typeof(Lifx.Api.Test.Cloud.CloudLightStateGuard))]
