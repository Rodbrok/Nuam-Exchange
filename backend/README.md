# Backend NUAM Exchange

Backend inicial ASP.NET Core .NET 8 para la API real de Calificaciones. La solución usa arquitectura por capas: `NuamExchange.Domain`, `NuamExchange.Application`, `NuamExchange.Infrastructure` y `NuamExchange.Api`, más proyectos de pruebas.

## Requisitos
- .NET SDK 8.
- SQL Server LocalDB, Express o Developer para desarrollo.
- No guardar credenciales en el repositorio.

## Restaurar y ejecutar
```powershell
cd backend
dotnet tool restore
dotnet restore NuamExchange.sln
dotnet ef database update --project src/NuamExchange.Infrastructure --startup-project src/NuamExchange.Api
dotnet run --project src/NuamExchange.Api
```

`ASPNETCORE_ENVIRONMENT` solo puede ser `Development` o `Testing` hasta implementar autenticación real en el Prompt 011.

## Connection strings
La clave es `ConnectionStrings:DefaultConnection` y puede reemplazarse con `ConnectionStrings__DefaultConnection` o user-secrets:
```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=NuamExchangeDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True" --project src/NuamExchange.Api
```
Ejemplos válidos: LocalDB, SQL Server Express (`.\SQLEXPRESS`) o SQL Server Developer local. No usar contraseñas versionadas.

## Migraciones
Las migraciones no se aplican automáticamente por defecto (`Database:ApplyMigrationsOnStartup=false`). Para aplicar use `scripts/update-database.ps1` o `dotnet ef database update`.

## API
Implementa `/api/v1/classifications`, `/api/v1/classifications/catalogs`, `/api/v1/classifications/{id}`, creación, actualización, copia y eliminación. Usa ProblemDetails, ETag/RowVersion, CORS restringido, rate limiting y Correlation ID (`x-correlation-id`). Swagger está habilitado solo en Development con esquema Bearer documentado como pendiente.

## Health checks
- `/health/live`: proceso activo, no consulta SQL Server.
- `/health/ready`: valida conectividad a SQL Server y puede responder 503 si no hay servidor disponible.

## Frontend en modo API
```powershell
$env:VITE_DATA_SOURCE="api"
$env:VITE_API_BASE_URL="https://localhost:7001/api/v1"
npm run dev
```
Para HTTPS de desarrollo: `dotnet dev-certs https --trust`.

## Pruebas
```powershell
dotnet test NuamExchange.sln
```
Las pruebas de API usan entorno `Testing`; no deben depender de SQL Server externo.

## Limitaciones
No se implementan login real, JWT real, usuarios, roles persistidos, uploads, dashboard, reportes, auditoría automática ni respaldos reales. El borrado es físico en esta etapa; el borrado lógico se evaluará después.
