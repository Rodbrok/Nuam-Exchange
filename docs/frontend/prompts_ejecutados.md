# Prompts ejecutados

## Prompt 001 — Reinicio limpio de la base frontend

Resultados reales:

- Se partió del commit inicial disponible en el repositorio local.
- La rama local disponible inicialmente era `work`; no existía remoto `origin` configurado para obtener `main`.
- Se renombró la rama local base a `main` y se creó `codex/prompt-001-reinicio-frontend` desde ese contenido inicial.
- Se creó la base React + TypeScript + Vite con React Router, ESLint y CSS propio.
- Se implementó layout administrativo con sidebar, header, breadcrumbs, rutas y placeholders.
- No se implementó backend, API, autenticación real, JWT, persistencia, reportes reales ni procesamiento de archivos.
- Los mockups no están en el repositorio y no fueron agregados.

## Prompt 002 — Login y sesión simulada por roles

Objetivo: implementar una pantalla `/login` funcional para una sesión simulada en memoria con permisos por rol, sin backend real ni JWT.

Archivos creados:

- `src/app/session/SessionContext.ts`.
- `src/app/session/SessionProvider.tsx`.
- `src/app/session/useSession.ts`.
- `src/mocks/users.ts`.
- `src/pages/AccessDeniedPage.tsx`.
- `src/routes/ProtectedRoute.tsx`.
- `src/types/session.ts`.

Archivos modificados:

- `src/app/App.tsx`.
- `src/components/Header.tsx`.
- `src/components/Sidebar.tsx`.
- `src/pages/LoginPage.tsx`.
- `src/routes/AppRoutes.tsx`.
- `src/routes/navigation.ts`.
- `src/styles/global.css`.
- `README.md`.
- `docs/frontend/arquitectura_frontend.md`.
- `docs/frontend/prompts_ejecutados.md`.

Perfiles implementados:

- Administrador: acceso completo a módulos preparados.
- Analista Tributario: acceso operativo sin módulos administrativos.
- Supervisor: acceso de consulta y cargas sin creación ni edición de calificaciones ni módulos administrativos.

Protección de rutas: las rutas administrativas requieren sesión; las rutas no permitidas por rol redirigen a `/sin-acceso`; el fallback envía a `/login` sin sesión y a `/inicio` con sesión.

Permisos por rol: centralizados en `src/routes/navigation.ts` y reutilizados por rutas y sidebar.

Validaciones ejecutadas: `git diff --check`, `git status --short`, `npm install`, `npm run lint` y `npm run build`.

Limitaciones: la sesión es exclusivamente en memoria; al recargar se pierde. No existe autenticación real, API, JWT, cookies, `localStorage` ni `sessionStorage`.

Próximo paso: Prompt 003 — Listado de Calificaciones Tributarias.

## Prompt 003 — Listado de Calificaciones Tributarias

Objetivo: reemplazar el placeholder de `/calificaciones` por una pantalla funcional de consulta visual con datos mockeados, filtros, tabla administrativa, selección única, ordenamiento, paginación, estados visuales y acciones diferenciadas por rol.

Archivos creados:

- `src/types/classification.ts`.
- `src/mocks/classifications.ts`.
- `src/pages/ClassificationsPage.tsx`.
- `src/components/Button.tsx`.
- `src/components/DataTableShell.tsx`.
- `src/components/StatusBadge.tsx`.
- `src/components/EmptyState`, `LoadingState` y `ErrorState` dentro de `src/components/ViewStates.tsx`.
- `src/components/Pagination.tsx`.
- `src/components/FormField.tsx`.
- `src/components/InlineMessage.tsx`.

Archivos modificados:

- `src/routes/AppRoutes.tsx`.
- `src/styles/global.css`.
- `README.md`.
- `docs/frontend/arquitectura_frontend.md`.
- `docs/frontend/prompts_ejecutados.md`.

Funciones implementadas:

- Modelo `Classification` y tipos auxiliares de filtros, ordenamiento y paginación.
- Mock de 22 registros ficticios para probar mercados, orígenes, ejercicios, estados, instrumentos, factores, montos y varias páginas.
- Filtros en memoria aplicados por botón Buscar y restablecidos por Limpiar.
- Tabla densa con scroll horizontal, selección única, fila seleccionada, hover, badges de estado, montos `es-CL`, factor con hasta 6 decimales y acción Ver.
- Ordenamiento accesible por ejercicio, instrumento, fecha de pago, monto y estado.
- Paginación con página anterior, página siguiente, página actual, total de páginas y tamaños 5, 10 y 20.
- Barra de acciones visuales según rol activo sin modificar la sesión ni los permisos existentes.
- Estados loading, empty y error con selector temporal de demostración.

Validaciones: se deben ejecutar `git diff --check`, `git status --short`, `npm install`, `npm run lint` y `npm run build` al cerrar el prompt.

Limitaciones: no existe backend, API, persistencia, CRUD real, carga real de archivos ni almacenamiento en navegador. El estado error es una demostración temporal para validar la interfaz.

Próximo paso: Prompt 004 — Formularios de ingreso, edición y copia de Calificaciones.

## Prompt 004 — Formularios de ingreso, edición y copia de Calificaciones

Objetivo: implementar formularios compartidos para ingresar, modificar y copiar calificaciones tributarias de forma simulada, sin backend ni persistencia.

Archivos creados:

- `src/features/classifications/ClassificationForm.tsx`.
- `src/features/classifications/classificationFormConfig.ts`.
- `src/features/classifications/classificationFormUtils.ts`.
- `src/features/classifications/classificationValidation.ts`.
- `src/features/classifications/mockFactorCalculator.ts`.
- `src/pages/ClassificationCreatePage.tsx`.
- `src/pages/ClassificationEditPage.tsx`.
- `src/pages/ClassificationCopyPage.tsx`.

Archivos modificados:

- `src/pages/ClassificationsPage.tsx`.
- `src/routes/AppRoutes.tsx`.
- `src/routes/navigation.ts`.
- `src/styles/global.css`.
- `README.md`.
- `docs/frontend/arquitectura_frontend.md`.
- `docs/frontend/prompts_ejecutados.md`.

Funcionalidades: formulario reutilizable en modos create, edit y copy; carga breve para editar/copiar; registro no encontrado; navegación de copia desde el listado; confirmación accesible para descartar cambios; procesamiento y éxito simulados con resumen de valores.

Validaciones: opciones autorizadas, campos obligatorios, longitudes de instrumento, descripción y secuencia, secuencia alfanumérica con guion, fecha válida, monto positivo con dos decimales como máximo y factor calculado mayor que cero.

Limitaciones: no hay backend, API, persistencia, modificación de mocks, `fetch`, Axios, `localStorage` ni `sessionStorage`. El factor es referencial, ficticio y no tributario.

Próximo paso: Prompt 005 — Carga X Factor, Carga X Monto y formato de archivo.

## Prompt 005 — Carga X Factor, Carga X Monto y formato de archivo

**Objetivo:** reemplazar placeholders de cargas y plantillas por pantallas funcionales de demostración CSV.

**Archivos creados y modificados:** se agregaron tipos de upload, componentes reutilizables, parser, validación, utilidades CSV, permisos, mocks de revisión y páginas de carga/plantillas; se actualizaron rutas, estilos y documentación.

**Formatos provisionales:** X Factor usa `ejercicio;mercado;instrumento;fechaPago;secuenciaEvento;factorActualizacion`; X Monto usa `ejercicio;mercado;instrumento;fechaPago;secuenciaEvento;monto`.

**Validaciones:** extensión `.csv`, archivo no vacío, máximo 5 MB, máximo 1.000 filas, encabezados requeridos, columnas inesperadas/duplicadas, valores por fila, fechas reales, decimales y duplicados.

**Roles:** Administrador y Analista Tributario pueden seleccionar, validar, procesar y descargar; Supervisor solo revisa datos ficticios y descarga errores mockeados.

**Pruebas:** `git diff --check`, instalación, lint y build deben ejecutarse al cierre del prompt. Las pruebas manuales cubren archivos correctos, inválidos, duplicados, descargas, procesamiento, plantillas, roles y responsive.

**Limitaciones:** no hay backend, API, persistencia, Excel real ni contrato tributario definitivo.

**Próximo paso:** Prompt 006 — Dashboard, vista consolidada y reportes simulados.

## Prompt 006 — Dashboard, vista consolidada y reportes simulados

**Objetivo:** implementar Dashboard en `/inicio`, vista consolidada y reportes simulados en `/reportes`, sin backend ni persistencia.

**Archivos creados:** componentes y utilidades de `src/features/dashboard`, componentes y utilidades de `src/features/reports`, tipos `dashboard.ts` y `report.ts`, mock complementario `dashboardActivity.ts`, utilidad compartida `csvExport.ts` y página `ReportsPage.tsx`.

**Archivos modificados:** `InicioPage.tsx`, `AppRoutes.tsx`, `uploadParser.ts`, `uploadTemplateUtils.ts`, estilos globales, README y documentación frontend.

**Indicadores y gráficos:** métricas derivadas de mocks, distribución por estado y monto por mercado con barras CSS accesibles.

**Reportes y exportación:** reporte de Calificaciones y Cargas con filtros, validación de rango, ordenamiento, paginación y CSV seguro con BOM, punto y coma, escape de celdas y protección contra fórmulas.

**Corrección uploadParser:** se reemplazó `headers.forEach((h, idx) => {` por `headers.forEach((h) => {` cuando `idx` no se utilizaba, sin alterar la lógica del parser.

**Pruebas:** `git diff --check`, `npm ci`, `npm run lint` y `npm run build`.

**Limitaciones:** datos simulados en memoria; sin backend, API, persistencia ni detalle individual nuevo.

**Próximo paso:** Prompt 007 — Administración de usuarios, roles y permisos.

## Prompt 007 — Administración de usuarios, roles y permisos

- **Objetivo:** reemplazar placeholders administrativos por pantallas simuladas para usuarios, roles y permisos.
- **Archivos creados:** tipos de administración y permisos, mocks de usuarios administrativos, componentes reutilizables del módulo `administration` y páginas `UsersAdministrationPage` y `RolesPermissionsPage`.
- **Archivos modificados:** rutas principales, `StatusBadge`, estilos globales y documentación.
- **Funcionalidades:** consulta, filtros, ordenamiento, paginación, creación, edición, cambios de estado, restablecimiento simulado y exportación CSV de usuarios; selector de roles, resumen y matriz de permisos demostrativa.
- **Validaciones:** nombre obligatorio de 3 a 80 caracteres, correo válido normalizado a minúsculas, correo único, rol y estado obligatorios.
- **Permisos:** Administrador mantiene acceso total y bloqueado; Analista Tributario y Supervisor pueden editar copias locales demostrativas con dependencias sobre el permiso Ver.
- **Seguridad:** no se manejan contraseñas, tokens, JWT, secretos, fetch, Axios, localStorage ni sessionStorage; no se modifica la sesión activa ni ProtectedRoute.
- **Pruebas:** se ejecutaron `git diff --check`, `git status --short`, `npm ci`, `npm run lint` y `npm run build`.
- **Limitaciones:** sin backend, API, base de datos ni persistencia permanente; los cambios se pierden al recargar.
- **Próximo paso:** Prompt 008 — Auditoría, respaldos y cierre visual del frontend.

## Prompt 008 — Auditoría, respaldos y cierre visual del frontend

**Objetivo:** completar `/auditoria` y `/respaldos`, reemplazar las pantallas de reserva restantes y cerrar consistencia visual, accesibilidad, seguridad y documentación del frontend.

**Archivos creados:** tipos `audit` y `backup`, mocks de eventos y respaldos, módulos `features/audit`, `features/backups`, páginas `AuditPage` y `BackupsPage`, aviso `DemoNotice` y diálogo `ConfirmDialog`.

**Archivos modificados:** rutas, navegación, header, estilos globales, paginación, botón reutilizable, README y documentación frontend.

**Auditoría:** eventos ficticios distribuidos entre 2024, 2025 y 2026, filtros aplicados manualmente, resumen derivado, tabla con ordenamiento y paginación, detalle accesible y CSV seguro.

**Respaldos:** registros ficticios, creación y restauración simuladas con progreso en memoria, cancelación visual, manifiesto CSV y política editable sin persistencia.

**Seguridad:** no se implementó backend, API, persistencia, respaldos reales, restauración real, fetch, Axios, almacenamiento web ni secretos.

**Accesibilidad:** labels, errores con `role="alert"`, captions, `aria-sort`, diálogos modales, Escape, foco inicial, progreso accesible y botones disabled reales.

**Cierre visual:** se eliminó la pantalla de reserva para auditoría/respaldos, se agregó indicador de modo demostración y se agrupó visualmente la navegación sin cambiar permisos efectivos.

**Pruebas:** `git diff --check`, `git status --short`, `npm ci`, `npm run lint` y `npm run build`.

**Limitaciones:** los datos, progreso, política y acciones son únicamente simulados en memoria.

**Próximo paso:** Prompt 009 — Preparación de contratos API e integración con backend ASP.NET Core .NET 8.

## Prompt 009 — Contratos API e integración preparada para ASP.NET Core .NET 8

**Objetivo:** preparar el frontend para una futura Web API ASP.NET Core .NET 8 con SQL Server, ProblemDetails, roles, paginación, validación y cargas multipart, manteniendo `mock` como fuente predeterminada.

**Archivos creados:** `.env.example`, capa `src/api` con configuración, contratos, `ApiError`, `HttpClient`, servicios de Calificaciones, mappers, provider y hook; `DataSourceIndicator`; documentos `docs/api/openapi.yaml`, `docs/api/contratos_api.md` e `docs/api/integracion_aspnet_core.md`.

**Archivos modificados:** `src/app/App.tsx`, `src/pages/ClassificationsPage.tsx`, `src/layouts/AppLayout.tsx`, `src/styles/global.css`, `src/vite-env.d.ts`, `README.md`, `docs/frontend/arquitectura_frontend.md` y este registro.

**Contratos:** comunes, autenticación provisional, Calificaciones, Cargas, Dashboard, Reportes, Administración, Auditoría y Respaldos.

**HttpClient y servicios:** transporte centralizado con timeout/cancelación, ProblemDetails y adaptadores mock/HTTP. Calificaciones queda como piloto con listados y catálogos por servicio.

**Seguridad:** sin almacenamiento de tokens, sin secretos, sin logs de credenciales, sin backend real ni persistencia. Autorización efectiva queda para backend futuro.

**Pruebas:** `git diff --check`, `npm ci`, `npm run lint`, `npm run build`; validación manual de modo mock y modo API sin backend cuando el entorno lo permita.

**Limitaciones:** endpoints y DTOs son provisionales; formularios de escritura y demás módulos no migran aún a HTTP.

**Próximo paso:** Prompt 010 — Creación inicial del backend ASP.NET Core .NET 8 y SQL Server.
