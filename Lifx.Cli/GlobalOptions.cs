using System.CommandLine;

namespace Lifx.Cli;

internal static class GlobalOptions
{
    /// <summary>
    /// Gets or sets Token.
    /// </summary>
    public static Option<string?> Token { get; } = new("--token", "-t")
    {
        Description = "LIFX Cloud API token (overrides stored credential)",
        Recursive = true
    };

    /// <summary>
    /// Gets or sets Verbose.
    /// </summary>
    public static Option<bool> Verbose { get; } = new("--verbose", "-v")
    {
        Description = "Enable verbose output with detailed information",
        Recursive = true
    };
}
