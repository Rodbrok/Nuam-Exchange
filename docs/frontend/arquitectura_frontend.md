# Arquitectura frontend

## Arquitectura React + TypeScript + Vite

El proyecto utiliza React con TypeScript sobre Vite. La entrada principal está en `src/main.tsx`, que monta la aplicación `src/app/App.tsx` y registra los estilos globales.

## Organización de carpetas

- `src/app`: composición raíz de la aplicación.
- `src/components`: componentes reutilizables del layout y páginas.
- `src/layouts`: layout administrativo principal.
- `src/pages`: páginas concretas como login e inicio.
- `src/routes`: definición de rutas y navegación.
- `src/styles`: variables CSS y estilos globales.
- `src/types`: tipos compartidos.

## Sistema de rutas

Las rutas se administran con `react-router-dom`. Existe una ruta visual `/login`, rutas administrativas bajo `AppLayout` y una ruta fallback que redirige a `/inicio`.

## Layout administrativo

El layout incluye sidebar lateral, header superior, breadcrumbs y área principal. El sidebar es colapsable en escritorio, desplegable en móvil y usa `NavLink` para estado activo.

## CSS propio

El sistema visual se define sin Tailwind, Bootstrap ni librerías de componentes. `tokens.css` contiene variables de color, fondo, superficies, bordes, texto, espaciados, tipografía, radios, sombras, sidebar, header y estados.

## Integración futura con ASP.NET Core

La integración con ASP.NET Core se realizará en próximos prompts. Por ahora no se hacen llamadas HTTP, no se usa `fetch`, no se agrega Axios y no existe autenticación real ni manejo de JWT.

## Mockups

Los mockups no están en el repositorio y no se suben como parte de esta base. El diseño se implementa desde la referencia textual de aplicación administrativa moderna.

## Sesión simulada en memoria

`SessionProvider` envuelve la aplicación dentro de `BrowserRouter` y expone el contexto de sesión a través de `useSession`. El contexto mantiene `user`, `status`, `isAuthenticated`, `login` y `logout` usando estado local de React. No existe persistencia en navegador: al recargar la página la sesión vuelve a estado anónimo.

Los datos de demostración viven en `src/mocks/users.ts` y contienen tres perfiles ficticios: Administrador, Analista Tributario y Supervisor. No se almacenan contraseñas, tokens ni credenciales reales.

## Protección de rutas y permisos

`ProtectedRoute` centraliza la validación de sesión activa y permisos por rol. Si no hay sesión, las rutas administrativas redirigen a `/login`. Si existe sesión pero el rol no tiene permiso para una ruta, la navegación redirige a `/sin-acceso`, donde se muestra una pantalla explícita de acceso restringido.

La matriz de permisos se define en `src/routes/navigation.ts` mediante `rolePermissions`. La misma matriz alimenta la protección de rutas y el filtrado del sidebar, evitando duplicar reglas en componentes visuales.

## Matriz de permisos actual

- Administrador: `/inicio`, `/calificaciones`, `/calificaciones/nueva`, `/calificaciones/:id/editar`, `/cargas/x-factor`, `/cargas/x-monto`, `/plantillas-carga`, `/reportes`, `/administracion/usuarios`, `/administracion/roles-permisos`, `/auditoria` y `/respaldos`.
- Analista Tributario: `/inicio`, `/calificaciones`, `/calificaciones/nueva`, `/calificaciones/:id/editar`, `/cargas/x-factor`, `/cargas/x-monto`, `/plantillas-carga` y `/reportes`.
- Supervisor: `/inicio`, `/calificaciones`, `/cargas/x-factor`, `/cargas/x-monto`, `/plantillas-carga` y `/reportes`.

## Limitaciones del login actual

El login es una simulación de frontend. No realiza llamadas HTTP, no valida contra backend, no usa JWT, no persiste datos y no documenta contraseñas fijas. El valor escrito en el campo de contraseña se usa únicamente para validar que no esté vacío durante el envío del formulario.

## Listado de Calificaciones Tributarias

El módulo `/calificaciones` usa la página `ClassificationsPage` y reemplaza el placeholder por una consulta administrativa con información mockeada. El modelo compartido `Classification` define `id`, `mercado`, `origen`, `ejercicio`, `instrumento`, `fechaPago`, `descripcion`, `secuenciaEvento`, `factorActualizacion`, `monto` y `estado`, junto con tipos auxiliares para filtros, ordenamiento y paginación.

Los datos ficticios viven en `src/mocks/classifications.ts`. Cubren mercados Acciones, Renta Fija y Fondos; orígenes Manual, Carga X Factor y Carga X Monto; ejercicios 2024, 2025 y 2026; estados Vigente, Pendiente, Observada y Rechazada; factores y montos variables. No representan información real ni privada.

El flujo de filtros separa valores en edición y valores aplicados. La tabla no filtra al escribir: el botón Buscar aplica los criterios en memoria y vuelve a la primera página; Limpiar restaura los valores iniciales y también vuelve a la primera página. El texto libre busca en instrumento, descripción y secuencia de evento.

El ordenamiento se resuelve sin librerías externas por ejercicio, instrumento, fecha de pago, monto y estado. El primer clic deja la columna en ascendente y el segundo en descendente; el estado se mantiene al cambiar de página y se expone con `aria-sort`.

La paginación reutilizable recibe estado tipado, total de registros, cambio de página y cambio de tamaño. Permite 5, 10 y 20 registros por página, deshabilita navegación en extremos y muestra el rango visible.

Las acciones principales respetan los roles ya existentes sin modificar la matriz de permisos. Administrador y Analista Tributario pueden ver acciones visuales de ingresar, modificar, eliminar, copiar, cargas y opciones. Supervisor no ve ingresar, modificar, eliminar ni copiar, pero sí puede consultar, seleccionar registros, acceder a cargas y usar opciones. Eliminar y copiar no ejecutan persistencia; solo muestran mensajes accesibles.

Los estados de vista son componentes reutilizables: `LoadingState` simula una carga breve al entrar y cancela su temporizador al desmontar, `EmptyState` aparece cuando no hay resultados filtrados y `ErrorState` se activa por un selector temporal llamado Estado de demostración. Estos estados no hacen llamadas HTTP y serán reemplazados o conectados a la API en prompts posteriores.

## Formularios compartidos de Calificaciones

El módulo de calificaciones incorpora `ClassificationForm` como formulario compartido para los modos `create`, `edit` y `copy`. Las páginas concretas se limitan a resolver valores iniciales, estado de carga breve y registros inexistentes; la lógica de edición de campos, envío simulado, validación, resumen de errores, confirmación de descarte y resultado de éxito vive en la carpeta funcional `src/features/classifications`.

La validación está separada en `classificationValidation.ts` y cubre obligatoriedad, opciones autorizadas, longitudes, formato de secuencia, fecha válida, monto positivo con máximo dos decimales y factor mayor que cero. Las funciones de fechas en `classificationFormUtils.ts` convierten entre el formato de los mocks `dd-MM-yyyy` y el formato de `input date` `yyyy-MM-dd`, rechazando fechas inválidas sin generar `Invalid Date` visible.

El factor se calcula en `mockFactorCalculator.ts` usando un catálogo ficticio por ejercicio y mes. Es una simulación frontend referencial, no una regla tributaria real, no proviene de API oficial y deberá reemplazarse por reglas del backend cuando exista integración.

Las rutas `/calificaciones/nueva`, `/calificaciones/:id/editar` y `/calificaciones/:id/copiar` se protegen con la matriz centralizada de `rolePermissions`. Administrador y Analista Tributario pueden acceder a las tres rutas; Supervisor queda redirigido a `/sin-acceso`. Los estados contemplados son carga inicial en editar/copiar, formulario listo, procesamiento, éxito simulado, errores de validación y registro no encontrado. No existe persistencia ni modificación permanente de mocks.

## Módulo uploads — Prompt 005

El módulo `src/features/uploads` concentra la demostración de cargas masivas para X Factor y X Monto. Las páginas reutilizan una misma pantalla base y cambian su comportamiento mediante configuraciones centralizadas.

- `uploadConfigs.ts` define columnas canónicas, ejemplos, reglas visibles y mensajes de formato provisional.
- `uploadParser.ts` implementa un parser CSV propio sin dependencias externas, con detección de coma/punto y coma, BOM UTF-8, comillas, comillas escapadas y líneas vacías ignoradas.
- `uploadValidation.ts` separa validación de archivo, estructura, encabezados y filas. Las filas duplicadas se detectan por ejercicio, mercado, instrumento, fecha de pago y secuencia de evento.
- `uploadTemplateUtils.ts` genera plantillas y reportes de errores CSV con escapes y mitigación de fórmulas de planilla.
- `uploadPermissions.ts` centraliza permisos: Administrador y Analista Tributario procesan; Supervisor revisa mocks.
- `UploadReviewPanel` usa `src/mocks/uploadReview.ts` con datos ficticios para la vista de Supervisor.

No existe backend, API, persistencia, `fetch`, Excel real ni almacenamiento local. La integración futura con backend deberá reemplazar el procesamiento simulado por contratos formales de carga, manteniendo las validaciones frontend como apoyo y no como control definitivo.

## Prompt 006: Dashboard, vista consolidada y reportes simulados

El módulo `src/features/dashboard` centraliza presentación y cálculo del Dashboard. `dashboardUtils.ts` filtra calificaciones por ejercicio/mercado, calcula indicadores desde mocks y arma series para gráficos accesibles. Los gráficos usan barras CSS, etiquetas visibles, porcentajes y alternativa textual; no incorporan canvas ni librerías externas.

El módulo `src/features/reports` contiene filtros, selector de tipo, resúmenes, tablas, ordenamiento, paginación y exportación. Los filtros se aplican solo al generar reporte; las fechas validan que desde no sea posterior a hasta y exponen errores con `role="alert"`. Las tablas tienen caption, `aria-sort`, scroll horizontal y paginación reutilizada.

La exportación CSV segura vive en `src/utils/csvExport.ts` y es reutilizada por reportes y plantillas/cargas. Genera UTF-8 con BOM, separador punto y coma, escape de comillas, neutralización de fórmulas (`=`, `+`, `-`, `@`) y revocación de object URL.

Permisos por rol: todos acceden a `/inicio` y `/reportes`. Administrador ve acciones administrativas, Analista Tributario ve acciones operativas y Supervisor solo acciones de consulta/revisión. La integración futura con backend debería sustituir mocks por endpoints protegidos sin cambiar la separación entre utilidades y componentes.

## Prompt 007 — Módulo `administration`

El módulo `src/features/administration` concentra la administración simulada de usuarios, roles y permisos. El modelo `AdministrationUser` define `id`, `nombre`, `email`, `rol`, `estado`, `fechaCreacion`, `ultimoAcceso` y `creadoPor`; sus estados permitidos son Activo, Inactivo y Bloqueado. Las validaciones viven en `administrationValidation.ts` y normalizan correos a minúsculas, exigen nombre de 3 a 80 caracteres, formato de correo y unicidad excluyendo el usuario editado.

`UsersAdministrationPage` mantiene el estado en memoria para filtros aplicados, borradores, ordenamiento, paginación, usuarios, diálogos y mensajes. Los diálogos React reemplazan confirmaciones nativas, usan `role="dialog"`, `aria-modal`, cierre con Escape y mensajes accesibles. La sesión activa queda protegida: no puede cambiar su propio rol ni estado desde la simulación y no se modifica el `SessionProvider`.

El catálogo tipado de permisos vive en `permissionCatalog.ts` y define módulos, acciones, permisos predeterminados por rol y dependencias. `RolesPermissionsPage` edita una copia local de la matriz para Analista Tributario y Supervisor; Administrador se muestra bloqueado como rol de control total. Esta matriz es únicamente visual: la seguridad efectiva continúa en `ProtectedRoute` y deberá integrarse con validaciones y persistencia del backend futuro.

## Prompt 008: Auditoría, respaldos y cierre visual

El módulo `src/features/audit` implementa la consulta administrativa simulada de auditoría. El modelo `AuditEvent` define identificador, fecha, usuario, rol, módulo, acción, entidad, resultado, severidad, origen y detalles seguros como pares etiqueta-valor. `auditUtils.ts` separa filtros, ordenamiento, validación de rango y resumen; `auditExport.ts` reutiliza `csvExport.ts` para exportar CSV con BOM, punto y coma, escape y protección contra fórmulas.

El módulo `src/features/backups` implementa la gestión visual de respaldos. El modelo `BackupRecord` contiene metadatos de respaldo, tipo, alcance, tamaño, estado, ejecutor, origen, registros y observaciones seguras. La simulación de progreso vive en memoria de `BackupsPage`, usa temporizadores limpiados por efecto y solo actualiza registros mockeados locales. `BackupPolicyPanel` edita una copia local de la política visual sin crear tareas programadas ni persistir configuración.

Los diálogos de auditoría, creación, restauración, manifiesto y confirmación declaran `role="dialog"`, `aria-modal`, foco inicial y cierre por Escape. Las tablas incluyen caption, `aria-sort`, scroll horizontal y paginación. Los permisos efectivos de `/auditoria` y `/respaldos` siguen centralizados en `ProtectedRoute` y `navigation.ts`, únicamente para Administrador.

Seguridad: no se crean APIs, backend, base de datos, archivos de respaldo, ZIP, tareas programadas ni restauraciones reales. No se usan `fetch`, Axios, almacenamiento web, `dangerouslySetInnerHTML`, secretos ni rutas de infraestructura. Para la integración futura con ASP.NET Core .NET 8 se esperan servicios de auditoría paginada, exportación server-side, catálogo de respaldos, ejecución de trabajos controlados, manifiestos y política persistente con autorización real por rol.
