$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
  dotnet tool restore
  dotnet ef database update --project src/NuamExchange.Infrastructure --startup-project src/NuamExchange.Api
} finally { Pop-Location }
