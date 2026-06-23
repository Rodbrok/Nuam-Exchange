import React from 'react';
import ReactDOM from 'react-dom/client';
import { Navigate, RouterProvider, createBrowserRouter } from 'react-router-dom';

import { AppLayout } from './components/AppLayout';
import { SimplePage } from './components/SimplePage';
import { EditarCalificacionPage } from './pages/EditarCalificacionPage';
import { InicioPage } from './pages/InicioPage';
import { LoginPage } from './pages/LoginPage';
import './styles.css';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Navigate to="/login" replace />,
  },
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    element: <AppLayout />,
    children: [
      {
        path: '/inicio',
        element: <InicioPage />,
      },
      {
        path: '/carga-x-factor',
        element: <SimplePage title="Carga X Factor" />,
      },
      {
        path: '/carga-x-monto',
        element: <SimplePage title="Carga X Monto" />,
      },
      {
        path: '/plantillas-carga',
        element: <SimplePage title="Plantillas de carga" />,
      },
      {
        path: '/reportes',
        element: <SimplePage title="Reportes" />,
      },
      {
        path: '/calificaciones/:id/editar',
        element: <EditarCalificacionPage />,
      },
      {
        path: '/administracion',
        element: <SimplePage title="Administración" />,
      },
      {
        path: '/usuarios',
        element: <SimplePage title="Usuarios" />,
      },
      {
        path: '/roles-permisos',
        element: <SimplePage title="Roles y permisos" />,
      },
      {
        path: '/respaldos',
        element: <SimplePage title="Respaldos" />,
      },
      {
        path: '/auditoria',
        element: <SimplePage title="Auditoría" />,
      },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
);
