# Azure container image updater

When new container image is published with changing label like `latest`, `latest-master` (or similar) in development environments it
is useful that matching images like `foo:latest` on publish of new image `foo:latest` will be automatically updated to running applications.

This middleman removes requirement that builds have to understand locations where image are actually running: they can simply call globally
update all matching images in all environments.

## Starting development

You need dotnet SDK <https://dotnet.microsoft.com/download> and <https://github.com/Azure/azure-functions-core-tools>.

Copy [./local.settings.template.json](./local.settings.template.json) to `local.settings.json` and configure it so it can access desired subscription.

Then start software locally:

```powershell
dotnet build
func start
```

Function runtime prints localhost address where functions are running. Use that address to test image updating locally:

```powershell
Invoke-RestMethod https://localhost:1234/api/update -Method Post -Body (@{ ImageName = "ptcos/pdf-storage"; Tag = "latest" } | ConvertTo-Json) -Headers @{Authorization = "ApiKey apikeyhere"}
```

## Deployment

Powershell (Core) and [Az Powershell](https://docs.microsoft.com/en-us/powershell/azure/new-azureps-module-az).

Then you can deploy function and required infrastructure with:

```powershell
./Deployment/Deploy.ps1 -ResourceGroup "my-resource-group" -ApiKey "myapikey"
```
