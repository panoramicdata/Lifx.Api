# Copilot Instructions

## General Guidelines
- Target .NET 10 for all projects in this solution
- Use C# 14.0 language features where appropriate
- Prefer async/await patterns throughout the codebase
- Keep models as POCOs with no external dependencies

## Code Style
- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Enable implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
- Treat warnings as errors in all projects
- Use snake_case for JSON property names (matching LIFX API conventions)
- Follow existing serialization patterns using `System.Text.Json`

## Testing
- Use xUnit v3 for unit tests
- Use AwesomeAssertions for fluent assertions
- Store API keys as user secrets, never in source code
- The test project UserSecretsId is `b19ce93c-3048-4afa-a755-fa414053b674`
- Set the LIFX API key with: `dotnet user-secrets set "LifxApiKey" "<your-key>"`

## Publishing
- Use the `release.ps1` script for publishing NuGet packages
- The NuGet API key is stored locally in `nuget-key.txt` (not committed to repo)
- Packages are published to NuGet.org
- Symbol packages (`.snupkg`) are generated automatically

## Project Structure
- `Lifx.Api` - Main library with Cloud HTTP API and LAN protocol support
- `Lifx.Api.Test` - Unit and integration tests
- `Lifx.Cli` - Command-line interface tool

## GitHub Integration
- Use `gh issue view <number>` command to get issue details instead of browser navigation
- Use `gh pr` commands for pull request operations