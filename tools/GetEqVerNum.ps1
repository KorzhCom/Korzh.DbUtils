
$config = Get-Content ".\buildsettings.json" | Out-String | ConvertFrom-Json

$assemblyVersion = $config.assemblyVersion
Write-Host "##vso[task.setvariable variable=version.assembly]$assemblyVersion"
Write-Host "Assembly version: $assemblyVersion"

$packageVersion = $config.packageVersion
Write-Host "##vso[task.setvariable variable=version.package]$packageVersion"
Write-Host "Package version: $packageVersion"

$assetVersion = $config.assetVersion
Write-Host "##vso[task.setvariable variable=version.asset]$assetVersion"
Write-Host "Asset version: $assetVersion"

