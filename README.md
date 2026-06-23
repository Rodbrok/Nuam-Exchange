# Nuam Exchange Frontend

Frontend administrativo interno para el Sistema de Gestión Tributaria de Nuam Exchange, orientado a la gestión futura de calificaciones, cargas, reportes, usuarios, roles, auditoría y respaldos.

## Tecnologías

- React 19.
- React DOM 19.
- TypeScript 5.
- Vite 6.
- React Router DOM 7.
- ESLint 9.
- CSS propio sin frameworks visuales.

## Requisitos

- Node.js compatible con Vite 6.
- npm.

## Instalación

```bash
npm install
```

## Ejecución

```bash
npm run dev
```

## Comandos disponibles

- `npm run dev`: inicia Vite en modo desarrollo.
- `npm run build`: ejecuta TypeScript y genera el build de producción.
- `npm run lint`: ejecuta ESLint sobre el proyecto.
- `npm run preview`: sirve localmente el build generado.

## Estructura real

```text
src/
  app/
  components/
  layouts/
  pages/
  routes/
  styles/
  types/
docs/
  frontend/
```

## Estado actual

Esta base incluye rutas navegables, layout administrativo, sidebar responsive, header, breadcrumbs, páginas placeholder y sistema visual CSS propio. No existe autenticación real, persistencia, procesamiento de archivos ni reportes reales.

## Integración con API

Actualmente no existe integración con API, backend, JWT ni base de datos. La variable `VITE_API_BASE_URL` queda documentada en `.env.example` para una integración futura.
