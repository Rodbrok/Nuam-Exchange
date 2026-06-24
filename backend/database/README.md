# Base de datos

Las migraciones EF Core en `src/NuamExchange.Infrastructure/Migrations` son la fuente de verdad del esquema.

## Crear o actualizar en desarrollo
```powershell
cd backend
dotnet tool restore
dotnet ef database update --project src/NuamExchange.Infrastructure --startup-project src/NuamExchange.Api
```

## Script idempotente
```powershell
dotnet ef migrations script --idempotent --project src/NuamExchange.Infrastructure --startup-project src/NuamExchange.Api
```

## Revertir en desarrollo
Use `dotnet ef database update <MigracionAnterior>` solo sobre bases locales de desarrollo. No ejecutar contra producción; producción queda fuera de alcance hasta completar seguridad y operación.

No guardar credenciales ni connection strings con usuario/contraseña en el repositorio.
