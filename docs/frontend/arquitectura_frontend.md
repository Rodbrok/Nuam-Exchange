# Arquitectura frontend

## Estado actual

La base técnica actual usa React, TypeScript, Vite y React Router para vistas administrativas estáticas.

## Estructura versionada

- `src/main.tsx`: rutas principales, incluyendo `/calificaciones/:id/editar`.
- `src/components/AppLayout.tsx`: navegación visual por rol sin autorización real.
- `src/components/SimplePage.tsx`: contenedor reutilizable para vistas estáticas.
- `src/pages/LoginPage.tsx`: pantalla de acceso visual que navega internamente a `/inicio`.
- `src/pages/InicioPage.tsx`: pantalla inicial.
- `src/pages/EditarCalificacionPage.tsx`: pantalla estática para validar la ruta parametrizada.

No existen carpetas `features` versionadas en esta etapa.
