# Nuam Exchange Frontend

Frontend administrativo para el Sistema de Gestión Tributaria de Nuam Exchange, orientado a una corredora de bolsa.

## Tecnologías

- React + TypeScript + Vite.
- React Router DOM.
- CSS propio con variables.
- Lucide React disponible para iconografía futura.

## Requisitos mínimos

- Node.js 20 o superior recomendado.
- npm 10 o superior recomendado.

## Instalación y ejecución

```bash
npm install
npm run dev
```

## Validaciones

```bash
npm run lint
npm run build
```

## Estructura principal

- `src/app`: arranque de la aplicación.
- `src/routes`: rutas y navegación centralizada.
- `src/layouts`: layout administrativo, header, sidebar y breadcrumbs.
- `src/components`: componentes reutilizables.
- `src/pages`: páginas navegables.
- `src/mocks`: datos ficticios de usuarios y calificaciones.
- `src/api`: espacio reservado para integración futura con ASP.NET Core Web API.
- `src/styles`: sistema visual CSS.
- `docs/frontend`: documentación técnica del frontend.

## Estado actual

La interfaz es navegable, responsiva y utiliza datos mockeados. Aún no existe integración con API real, autenticación real, backend, base de datos ni endpoints simulados.
