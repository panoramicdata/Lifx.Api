# Pre-Publish Checklist

Use this checklist before running `Publish.ps1` to ensure a smooth release.

## Code Quality

- [ ] All code changes committed
- [ ] All tests passing locally
- [ ] Code reviewed (if applicable)
- [ ] No debug/console statements left in code
- [ ] XML documentation complete for public APIs
- [ ] README.md updated (if needed)

## Version Control

- [ ] On main/master branch (or confirm intended branch)
- [ ] Git working directory clean (`git status` shows no changes)
- [ ] Latest changes pulled from remote (`git pull`)
- [ ] No merge conflicts
- [ ] Version incremented (if needed) in `version.json`

## Testing

- [ ] All unit tests passing (`dotnet test --filter "Unit"`)
- [ ] All LAN tests passing (`dotnet test --filter "Lan"`)
- [ ] Integration tests reviewed (may require hardware)
- [ ] No skipped tests without good reason
- [ ] Test coverage acceptable (target: 92%+)

## Documentation

- [ ] README.md reflects current features
- [ ] CHANGELOG.md updated (if exists)
- [ ] Breaking changes documented
- [ ] Migration guide provided (if breaking changes)
- [ ] Code examples tested and working

## Package Content

- [ ] .csproj package metadata correct
  - [ ] PackageId
  - [ ] Authors
  - [ ] Description
  - [ ] PackageTags
  - [ ] PackageLicenseExpression
  - [ ] RepositoryUrl
- [ ] Package icon set (if applicable)
- [ ] Dependencies correct
- [ ] Target frameworks correct

## NuGet Preparation

- [ ] NuGet API key ready in `nuget-key.txt`
- [ ] API key not expired
- [ ] API key has correct permissions (push to Lifx.Api)
- [ ] Package name not already taken (first publish only)

## Build Verification

- [ ] Clean build succeeds (`dotnet build --configuration Release`)
- [ ] No build warnings
- [ ] Package builds locally (`dotnet pack`)
- [ ] Package size reasonable (check nupkgs folder)

## Prerequisites Installed

- [ ] PowerShell 7.0+ (`pwsh --version`)
- [ ] .NET 10.0 SDK (`dotnet --version`)
- [ ] nbgv tool (`nbgv --version`)
- [ ] Git (`git --version`)

## Final Checks

- [ ] Release notes prepared
- [ ] Communication plan ready (if major release)
- [ ] Support channels prepared for questions
- [ ] Rollback plan considered
- [ ] Time allocated for post-publish monitoring

## Ready to Publish!

Once all items are checked:

```powershell
.\Publish.ps1
```

## Post-Publish Tasks

After successful publish:

- [ ] Verify package on NuGet.org
- [ ] Create GitHub release with notes
- [ ] Update project wiki/docs (if applicable)
- [ ] Announce in appropriate channels
- [ ] Monitor for issues/feedback
- [ ] Respond to initial user questions

## Emergency Rollback

If critical issue found after publish:

1. **Unlist package** (don't delete - breaks existing users)
   - Go to https://www.nuget.org/packages/Lifx.Api
   - Click "Manage Package"
   - Unlist the version

2. **Fix issue** in code

3. **Publish new patch version** with fix

4. **Communicate** the issue and fix to users
