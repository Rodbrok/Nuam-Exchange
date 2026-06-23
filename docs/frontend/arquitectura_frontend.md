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
