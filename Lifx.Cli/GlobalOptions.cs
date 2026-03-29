using System.CommandLine;

namespace Lifx.Cli;

internal static class GlobalOptions
{
    public static Option<string?> Token { get; } = new("--token", "-t")
    {
        Description = "LIFX Cloud API token (overrides stored credential)",
        Recursive = true
    };

    public static Option<bool> Verbose { get; } = new("--verbose", "-v")
    {
        Description = "Enable verbose output with detailed information",
        Recursive = true
    };
}
