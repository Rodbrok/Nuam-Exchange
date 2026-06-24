$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
  $env:ASPNETCORE_ENVIRONMENT = "Development"
  dotnet run --project src/NuamExchange.Api
} finally { Pop-Location }
