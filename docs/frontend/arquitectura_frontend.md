# Arquitectura frontend

Nuam Exchange se implementa con React, TypeScript estricto y Vite para construir una base administrativa navegable y mantenible.

## Organización de carpetas

El código se separa en `app`, `routes`, `layouts`, `components`, `pages`, `mocks`, `types`, `styles`, `api` y `features`. Las carpetas de features agrupan el alcance futuro por dominio funcional sin incluir lógica vacía.

## Sistema de rutas

Las rutas están centralizadas en `src/routes/AppRoutes.tsx` y cubren login, inicio, calificaciones, cargas, reportes, administración, auditoría y respaldos. Las rutas incompletas usan placeholders estructurados.

## Layout administrativo

El layout incluye sidebar colapsable, header, breadcrumbs, área principal, usuario mockeado, rol actual y cierre de sesión visual. El sidebar se adapta a escritorio, tablet y móvil.

## Datos mockeados

Los usuarios y calificaciones están en `src/mocks`. No se usa localStorage para datos de negocio ni credenciales reales.

## Control visual por roles

La navegación se filtra visualmente para Administrador, Analista Tributario y Supervisor. No existe autenticación real ni protección de endpoints.

## API futura

`src/api` queda reservado para integrar posteriormente ASP.NET Core Web API. No se creó fetch, axios, servicios falsos ni endpoints inventados.

## Criterio visual

La interfaz sigue un criterio de aplicación administrativa inspirada en programas de escritorio Windows modernizados: paneles sobrios, controles compactos, filtros superiores, tablas densas, bordes finos y colores profesionales.

## Mockups

Los mockups originales no se encuentran en el repositorio y este avance no crea imágenes ni reemplazos visuales.
