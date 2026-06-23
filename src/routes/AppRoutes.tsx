import { Navigate, Route, Routes } from 'react-router-dom';
import { AppLayout } from '../layouts/AppLayout';
import { AccessDeniedPage } from '../pages/AccessDeniedPage';
import { InicioPage } from '../pages/InicioPage';
import { LoginPage } from '../pages/LoginPage';
import { PlaceholderPage } from '../components/PlaceholderPage';
import { ClassificationsPage } from '../pages/ClassificationsPage';
import { ClassificationCreatePage } from '../pages/ClassificationCreatePage';
import { ClassificationEditPage } from '../pages/ClassificationEditPage';
import { ClassificationCopyPage } from '../pages/ClassificationCopyPage';
import { ProtectedRoute } from './ProtectedRoute';
import { useSession } from '../app/session/useSession';

export function AppRoutes() {
  const { isAuthenticated } = useSession();
  const fallbackPath = isAuthenticated ? '/inicio' : '/login';

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route path="/sin-acceso" element={<AccessDeniedPage />} />
          <Route element={<ProtectedRoute routePath="/inicio" />}>
            <Route path="/inicio" element={<InicioPage />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/calificaciones" />}>
            <Route path="/calificaciones" element={<ClassificationsPage />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/calificaciones/nueva" />}>
            <Route path="/calificaciones/nueva" element={<ClassificationCreatePage />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/calificaciones/:id/editar" />}>
            <Route path="/calificaciones/:id/editar" element={<ClassificationEditPage />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/calificaciones/:id/copiar" />}>
            <Route path="/calificaciones/:id/copiar" element={<ClassificationCopyPage />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/cargas/x-factor" />}>
            <Route path="/cargas/x-factor" element={<PlaceholderPage title="Carga X Factor" description="Módulo futuro para carga de archivo X Factor." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/cargas/x-monto" />}>
            <Route path="/cargas/x-monto" element={<PlaceholderPage title="Carga X Monto" description="Módulo futuro para carga de archivo X Monto." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/plantillas-carga" />}>
            <Route path="/plantillas-carga" element={<PlaceholderPage title="Plantillas de carga" description="Administración futura de plantillas de carga." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/reportes" />}>
            <Route path="/reportes" element={<PlaceholderPage title="Reportes" description="Consulta futura de reportes tributarios." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/administracion/usuarios" />}>
            <Route path="/administracion/usuarios" element={<PlaceholderPage title="Usuarios" description="Administración futura de usuarios." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/administracion/roles-permisos" />}>
            <Route path="/administracion/roles-permisos" element={<PlaceholderPage title="Roles y Permisos" description="Administración futura de roles y permisos." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/auditoria" />}>
            <Route path="/auditoria" element={<PlaceholderPage title="Auditoría" description="Consulta futura de trazabilidad y auditoría." />} />
          </Route>
          <Route element={<ProtectedRoute routePath="/respaldos" />}>
            <Route path="/respaldos" element={<PlaceholderPage title="Respaldos" description="Gestión futura de respaldos." />} />
          </Route>
        </Route>
      </Route>
      <Route path="*" element={<Navigate to={fallbackPath} replace />} />
    </Routes>
  );
}
