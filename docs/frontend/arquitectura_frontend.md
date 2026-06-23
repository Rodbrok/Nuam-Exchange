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
