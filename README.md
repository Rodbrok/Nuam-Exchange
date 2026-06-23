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

## Sesión simulada por roles

La aplicación cuenta con un login de demostración para validar navegación y permisos sin backend real. La sesión se mantiene únicamente en memoria mediante React Context, por lo que se pierde al recargar la página o cerrar la pestaña.

Perfiles disponibles:

- Administrador: acceso a todos los módulos administrativos preparados.
- Analista Tributario: acceso a inicio, calificaciones, cargas, plantillas y reportes.
- Supervisor: acceso a inicio, calificaciones, cargas, plantillas y reportes, sin alta ni edición de calificaciones.

Esta implementación no crea credenciales reales, no guarda contraseñas, no utiliza `localStorage`, `sessionStorage`, cookies, API, JWT ni llamadas HTTP. El campo de contraseña es solamente visual y exige contenido no vacío para simular el flujo.

## Listado mockeado de Calificaciones Tributarias

La ruta `/calificaciones` reemplaza el placeholder por una consulta administrativa funcional con datos ficticios en memoria. Incluye filtros por mercado, origen, ejercicio, estado y texto libre; el texto busca en instrumento, descripción y secuencia de evento solamente al presionar **Buscar**. **Limpiar** restablece los filtros y vuelve a la primera página.

La tabla permite selección única de registros, ordenamiento por ejercicio, instrumento, fecha de pago, monto y estado, scroll horizontal, badges de estado y formato de montos `es-CL`. La paginación permite 5, 10 o 20 registros por página y muestra el rango visible.

Las acciones visuales se muestran según el rol activo: Administrador y Analista Tributario ven ingreso, modificación, eliminación, copia, cargas y opciones; Supervisor conserva consulta, selección, cargas y opciones. Las acciones de eliminación, copia y opciones solo muestran mensajes informativos; las rutas de ingreso, edición y cargas navegan a pantallas preparadas sin guardar información.

No existe backend, API, persistencia, `fetch`, Axios, CRUD real ni almacenamiento local para este listado. Los estados loading, empty y error son visuales; el error se activa desde un control temporal de demostración que será retirado al integrar la API.

## Formularios de ingreso, edición y copia de Calificaciones

Las rutas `/calificaciones/nueva`, `/calificaciones/:id/editar` y `/calificaciones/:id/copiar` usan un formulario compartido para simular ingreso, modificación y copia de calificaciones tributarias. El formulario valida campos en frontend, muestra errores accesibles junto a cada control, resume los errores al enviar y enfoca el primer campo inválido.

El factor de actualización se calcula automáticamente en memoria con un catálogo mock por ejercicio y mes. Es un cálculo referencial de demostración: no es una fórmula tributaria real, no debe usarse en producción y no proviene de una API oficial. La conversión de fechas entre `dd-MM-yyyy` y `yyyy-MM-dd` se realiza sin librerías externas.

Los envíos exitosos solo muestran un resumen visual de valores procesados. No se modifica el mock, no se persiste información, no se utiliza backend, no hay llamadas HTTP y no se usa `localStorage` ni `sessionStorage`.

## Cargas masivas CSV provisionales

El frontend incluye pantallas demostrativas para **Carga X Factor**, **Carga X Monto** y **Plantillas de carga**. El formato es provisional: el contrato definitivo será entregado por backend.

- Soporte real únicamente para archivos `.csv` seleccionados o arrastrados en el navegador, sin carga a servidor.
- Validación de extensión, archivo no vacío, tamaño máximo de 5 MB y máximo 1.000 filas de datos.
- Parser CSV propio con separador `;` o `,`, BOM UTF-8, saltos LF/CRLF, campos con comillas y comillas escapadas.
- Vista previa de hasta 100 filas, filtros por estado, resumen de registros válidos/con error y reporte CSV de errores.
- Procesamiento simulado con progreso: no realiza `fetch`, no guarda archivos y no persiste datos.
- Supervisor accede a una vista de revisión mockeada y no puede seleccionar ni procesar nuevos archivos.
- Las plantillas CSV se generan en el navegador con BOM UTF-8 y separador punto y coma.
