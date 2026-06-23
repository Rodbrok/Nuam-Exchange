# Contratos API provisionales

URL base: `/api/v1`. El versionado inicial queda en la ruta para permitir evolución controlada. JSON usa `camelCase`; fechas sin hora usan `yyyy-MM-dd` (`DateOnly` futuro) y fecha-hora usa ISO 8601 con zona (`DateTimeOffset`). Montos y factores son `decimal` en backend y `number` en frontend. Los IDs son `string` y `rowVersion` viaja en Base64 cuando exista concurrencia.

La paginación de servidor usa `page`, `pageSize`, `totalItems` y `totalPages`. Ordenamiento usa `sortBy` y `sortDirection` (`asc`/`desc`). Filtros viajan como query string. Errores usan `application/problem+json` compatible con ProblemDetails, incluyendo `traceId` y `errors` para validación. Cada solicitud frontend envía `x-correlation-id`. Actualizaciones y eliminaciones pueden enviar `If-Match` con `rowVersion`.

Las cargas usan `multipart/form-data` con `IFormFile` futuro. Seguridad: JWT Bearer provisional, sin almacenar tokens en localStorage/sessionStorage, sin registrar passwords ni Authorization. Los roles son provisionales y deberán alinearse con autorización del backend.

| Módulo | Endpoints provisionales |
|---|---|
| Auth | `POST /auth/login`, `POST /auth/logout`, `GET /auth/me` |
| Calificaciones | `GET/POST /classifications`, `GET /classifications/catalogs`, `GET/PUT/DELETE /classifications/{id}`, `POST /classifications/{id}/copy` |
| Cargas | `POST /uploads/x-factor/validate`, `POST /uploads/x-factor/process`, `POST /uploads/x-amount/validate`, `POST /uploads/x-amount/process`, `GET /uploads` |
| Dashboard | `GET /dashboard/summary` |
| Reportes | `GET /reports/classifications`, `GET /reports/uploads` |
| Usuarios | `GET/POST /users`, `PUT /users/{id}`, `PATCH /users/{id}/status`, `POST /users/{id}/reset-access` |
| Roles | `GET /roles`, `GET/PUT /roles/{role}/permissions` |
| Auditoría | `GET /audit-events` |
| Respaldos | `GET/POST /backups`, `POST /backups/{id}/restore`, `POST /backups/{id}/cancel`, `GET /backups/{id}/manifest`, `GET/PUT /backup-policy` |

Estados HTTP esperados: 200/201/202/204 para éxito; 400/422 validación; 401 sesión inválida; 403 permisos; 404 no encontrado; 409 concurrencia/conflicto; 500 error del servidor. El modo API puede fallar controladamente hasta que exista backend.

## Prompt 010 — Estado implementado

La API real de Calificaciones está implementada en ASP.NET Core .NET 8 bajo `backend/`. Están disponibles listado, catálogos, obtención por id, creación, actualización, copia y eliminación. Las respuestas se devuelven directamente, sin wrapper `data`, y los errores usan `application/problem+json` con `traceId` y `errors` cuando aplica.

`If-Match` es opcional en esta fase para `PUT` y `DELETE`; acepta RowVersion Base64 con o sin comillas y será obligatorio en una etapa posterior. La autenticación real, JWT, usuarios y roles quedan pendientes para Prompt 011. Por seguridad temporal, la API solo inicia en `Development` o `Testing`.
