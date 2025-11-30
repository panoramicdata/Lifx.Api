# User Secrets Configuration Guide

## What are User Secrets?

User Secrets is a secure way to store sensitive configuration data (like API tokens) during development without committing them to source control.

## Setting Up User Secrets

### Quick Start
```bash
# Navigate to the test project directory
cd Lifx.Api.Test

# Set your LIFX Cloud API token
dotnet user-secrets set "AppToken" "your-lifx-cloud-api-token-here"
```

### Get Your LIFX API Token
1. Go to https://cloud.lifx.com/settings
2. Generate a new token or copy your existing token
3. Use the command above to store it securely

## Viewing Your Secrets

```bash
# List all secrets
dotnet user-secrets list --project Lifx.Api.Test
```

## Secrets File Location

Your secrets are stored at:
- Windows: %APPDATA%\\Microsoft\\UserSecrets\\b19ce93c-3048-4afa-a755-fa414053b674\\secrets.json

## Benefits of User Secrets

- Keeps sensitive data out of source control  
- Easy to configure and manage  
- Per-developer configuration  
- Works across different machines  
- Integrated with .NET configuration system  
