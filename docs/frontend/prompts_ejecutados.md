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
