[CmdletBinding()]
Param(
    [Parameter(Mandatory)]$ResourceGroup
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function PublishFunction([string] $WebAppName)
{
    $publishFolder = Join-Path ([System.IO.Path]::GetTempPath()) ([Guid]::NewGuid())
    New-Item -ItemType Directory $publishFolder
    dotnet publish -c Release -o $publishFolder $PSScriptRoot/../Azure.Container.Updater.csproj

    $deployZip = [System.IO.Path]::GetTempFileName() + ".zip"

    Write-Host "Compressing $publishFolder\*"
    Compress-Archive -Path "$publishFolder\*" -DestinationPath $deployZip -Force

    Write-Host "Deploying $deployZip to $WebAppName in $ResourceGroup..."

    #$webApp = Get-AzWebApp -ResourceGroupName $ResourceGroup -Name $WebAppName
    #Publish-AzWebApp -WebApp $webApp -ArchivePath $fullZipTarget -Force

    Remove-Item $publishFolder -Recurse -Force
    Remove-Item $deployZip
}

PublishFunction