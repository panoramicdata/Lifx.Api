namespace Lifx.Api.Test;

/// <summary>
/// Exception to skip tests that can't run due to configuration or environment
/// </summary>
public class SkipException(string message) : Exception(message)
{
}
