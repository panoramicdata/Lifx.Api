# Publishing Guide

This document describes how to publish a new version of Lifx.Api to NuGet.org.

## Prerequisites

1. **PowerShell 7.0+**
   ```bash
   # Check version
   pwsh --version
   ```

2. **.NET 10.0 SDK**
   ```bash
   dotnet --version
   ```

3. **Nerdbank.GitVersioning CLI**
   ```bash
   dotnet tool install -g nbgv
   ```

4. **NuGet API Key**
   - Get your key from https://www.nuget.org/account/apikeys
   - Create `nuget-key.txt` in the repository root
   - Add your API key to this file (it's .gitignored)

## Publishing Process

### Automated (Recommended)

Simply run the publish script:

```powershell
.\Publish.ps1
```

The script will:
1. Check for clean git working directory (porcelain)
2. Verify all prerequisites are installed
3. Run all tests (must pass 100%)
4. Get version from Nerdbank.GitVersioning
5. Build the NuGet package
6. Create git tag (e.g., `v1.0.0`)
7. Publish to NuGet.org
8. Push git tag to remote

### Manual Steps

If you need to publish manually:

1. **Ensure clean git state**
   ```bash
   git status
   # Should show "nothing to commit, working tree clean"
   ```

2. **Run all tests**
   ```bash
   dotnet test --configuration Release
   ```

3. **Get version**
   ```bash
   nbgv get-version
   ```

4. **Build package**
   ```bash
   dotnet pack ./Lifx.Api/Lifx.Api.csproj --configuration Release --output ./nupkgs
   ```

5. **Create git tag**
   ```bash
   git tag -a v1.0.0 -m "Release 1.0.0"
   ```

6. **Publish to NuGet**
   ```bash
   dotnet nuget push ./nupkgs/Lifx.Api.1.0.0.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json
   ```

7. **Push tag to remote**
   ```bash
   git push origin v1.0.0
   ```

## Version Management

This project uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) for automatic semantic versioning based on git history.

### Incrementing Versions

Version is controlled by `version.json` in the repository root:

```json
{
  "$schema": "https://raw.githubusercontent.com/dotnet/Nerdbank.GitVersioning/master/src/NerdBank.GitVersioning/version.schema.json",
  "version": "1.0",
  "publicReleaseRefSpec": [
    "^refs/heads/main$",
    "^refs/heads/master$"
  ]
}
```

To increment the version:

1. **Patch version** (1.0.0 -> 1.0.1)
   - Automatic on each commit
   
2. **Minor version** (1.0.x -> 1.1.0)
   ```bash
   nbgv set-version 1.1
   git add version.json
   git commit -m "Increment minor version to 1.1"
   ```

3. **Major version** (1.x.x -> 2.0.0)
   ```bash
   nbgv set-version 2.0
   git add version.json
   git commit -m "Increment major version to 2.0"
   ```

### Version Format

- **Releases from main/master**: `1.0.0`
- **Pre-releases**: `1.0.0-alpha.{height}` (based on commit height)
- **Local builds**: `1.0.0-{branchName}.{height}`

## Troubleshooting

### "Git working directory is not clean"
- Commit or stash all changes before publishing
- Check with `git status`

### "Tests failed"
- Fix failing tests before publishing
- Run tests locally: `dotnet test`
- Check test output for specific failures

### "Tag already exists"
- Version needs to be incremented
- Either increment manually or make a new commit (auto-increments patch)

### "nuget-key.txt not found"
- Create the file in repository root
- Add your NuGet API key
- Verify file is .gitignored

### "Failed to publish to NuGet"
- Check API key is valid and not expired
- Verify package doesn't already exist with this version
- Check NuGet.org status: https://status.nuget.org/

### "Failed to push tag to remote"
- Check you have push permissions to the repository
- Manually push: `git push origin v1.0.0`

## Security Notes

- **Never commit `nuget-key.txt`** - It's .gitignored for security
- Keep your NuGet API key private
- Rotate keys periodically
- Use scoped keys (push-only for specific packages)

## Post-Publish

After successful publish:

1. Package appears on NuGet.org (may take a few minutes)
   - https://www.nuget.org/packages/Lifx.Api/

2. Update release notes on GitHub
   - https://github.com/panoramicdata/Lifx.Api/releases

3. Announce release (optional)
   - Update documentation
   - Social media
   - Blog posts

## CI/CD Integration

For automated publishing via GitHub Actions or Azure DevOps:

1. Store NuGet API key as a secret
2. Run `Publish.ps1` in CI pipeline
3. Only publish from main/master branch
4. Ensure all tests pass before publish

Example GitHub Actions workflow:

```yaml
name: Publish to NuGet

on:
  push:
    branches: [ main ]
    tags: [ 'v*' ]

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
        with:
          fetch-depth: 0
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Install nbgv
        run: dotnet tool install -g nbgv
      
      - name: Create nuget-key.txt
        run: echo "${{ secrets.NUGET_API_KEY }}" > nuget-key.txt
      
      - name: Publish
        run: pwsh ./Publish.ps1
```

## Support

For issues with publishing:
- Check this guide
- Review script output for errors
- Open an issue: https://github.com/panoramicdata/Lifx.Api/issues
