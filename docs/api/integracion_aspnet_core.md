# Integración futura con ASP.NET Core .NET 8

Se recomienda ASP.NET Core Web API .NET 8 con Controllers para mantener separación clara por módulo, filtros, autorización por acción y documentación Swagger legible. Minimal APIs siguen siendo viables, pero Controllers favorecen el crecimiento del dominio NUAM.

Usar DTOs separados de entidades, `System.Text.Json` en camelCase, ProblemDetails nativo, `CancellationToken` en todas las acciones, `DateOnly` para fechas sin hora, `DateTimeOffset` para auditoría, `decimal` para montos/factores e `IFormFile` para cargas. SQL Server con EF Core 8, consultas `AsNoTracking`, paginación desde servidor y `rowversion` para concurrencia con ETag/If-Match.

Seguridad: autorización por roles en backend, CORS restringido, HTTPS, rate limiting, logs estructurados sin datos sensibles, health checks y Swagger habilitado solo según ambiente.

```csharp
public sealed record PaginatedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems, int TotalPages);
public sealed record ClassificationDto(string Id, string Market, string Source, int FiscalYear, string Instrument, DateOnly PaymentDate, string Description, string EventSequence, decimal UpdateFactor, decimal Amount, string Status, string? RowVersion);
public sealed record CreateClassificationRequest(string Market, string Source, int FiscalYear, string Instrument, DateOnly PaymentDate, string Description, string EventSequence, decimal UpdateFactor, decimal Amount, string Status);
public sealed record ApiErrorMapping(string TraceId, ProblemDetails ProblemDetails);
```

Validación puede iniciarse con DataAnnotations o validación manual; FluentValidation podría evaluarse después sin imponer dependencia ahora. No confiar en validaciones frontend.

## Prompt 010 — Backend creado

Se creó la solución `backend/NuamExchange.sln` con proyectos `Domain`, `Application`, `Infrastructure`, `Api` y pruebas. La API usa EF Core 8 con SQL Server, migración `InitialCreate`, datos semilla temporales de Calificaciones, ProblemDetails, manejador global de excepciones, Correlation ID, CORS restringido, rate limiting, health checks y Swagger solo en Development.

Siguiente etapa: Prompt 011 implementará autenticación JWT, usuarios, roles y autorización en backend.
