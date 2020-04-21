[CmdletBinding()]
Param(
    [Parameter(Mandatory)][string]$ResourceGroup,
    [Parameter()][hashtable]$Tags
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Write-Host "Creating resource group $($ResourceGroup)"
New-AzResourceGroup -Name $ResourceGroup -Location "northeurope" -Force -Tag $Tags

function PublishFunction([string] $WebAppName) {
    $publishFolder = Join-Path ([System.IO.Path]::GetTempPath()) ([Guid]::NewGuid())
    New-Item -ItemType Directory $publishFolder
    dotnet publish -c Release -o $publishFolder $PSScriptRoot/../Azure.Container.Updater.csproj

    $deployZip = [System.IO.Path]::GetTempFileName() + ".zip"

    Write-Host "Compressing $publishFolder\*"
    Compress-Archive -Path "$publishFolder\*" -DestinationPath $deployZip -Force

    Write-Host "Deploying $deployZip to $WebAppName in $ResourceGroup..."

    $webApp = Get-AzWebApp -ResourceGroupName $ResourceGroup -Name $WebAppName
    Publish-AzWebApp -WebApp $webApp -ArchivePath $deployZip -Force

    Remove-Item $publishFolder -Recurse -Force
    Remove-Item $deployZip
}

Write-Host 'Creating environment...'
$functionAppOutputs = New-AzResourceGroupDeployment `
    -Name 'deployment-script' `
    -TemplateFile "$PsScriptRoot/azure-container-updater-app.json" `
    -ResourceGroupName $ResourceGroup `
    -appName $ResourceGroup `
    -ApiKey (ConvertTo-SecureString -String "abc" -AsPlainText -Force)

try {
    New-AzRoleAssignment -ObjectId $functionAppOutputs.Outputs.principalId.Value -RoleDefinitionName "Contributor" -Scope "/subscriptions/$($functionAppOutputs.Outputs.subscriptionId.Value)/" | Out-Null
}
catch {
    Write-Host "Failed to create new role assignment, this usually means that it already exist."
}

PublishFunction $functionAppOutputs.Outputs.appName.Value