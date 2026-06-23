import { Navigate, Route, Routes } from 'react-router-dom';
import { AppLayout } from '../layouts/AppLayout';
import { InicioPage } from '../pages/InicioPage';
import { LoginPage } from '../pages/LoginPage';
import { PlaceholderPage } from '../components/PlaceholderPage';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppLayout />}>
        <Route path="/inicio" element={<InicioPage />} />
        <Route path="/calificaciones" element={<PlaceholderPage title="Calificaciones Tributarias" description="Listado futuro de calificaciones tributarias." />} />
        <Route path="/calificaciones/nueva" element={<PlaceholderPage title="Nueva Calificación" description="Formulario futuro para registrar calificaciones tributarias." />} />
        <Route path="/calificaciones/:id/editar" element={<PlaceholderPage title="Editar Calificación" description="Ruta preparada para editar una calificación mediante parámetro." />} />
        <Route path="/cargas/x-factor" element={<PlaceholderPage title="Carga X Factor" description="Módulo futuro para carga de archivo X Factor." />} />
        <Route path="/cargas/x-monto" element={<PlaceholderPage title="Carga X Monto" description="Módulo futuro para carga de archivo X Monto." />} />
        <Route path="/plantillas-carga" element={<PlaceholderPage title="Plantillas de carga" description="Administración futura de plantillas de carga." />} />
        <Route path="/reportes" element={<PlaceholderPage title="Reportes" description="Consulta futura de reportes tributarios." />} />
        <Route path="/administracion/usuarios" element={<PlaceholderPage title="Usuarios" description="Administración futura de usuarios." />} />
        <Route path="/administracion/roles-permisos" element={<PlaceholderPage title="Roles y Permisos" description="Administración futura de roles y permisos." />} />
        <Route path="/auditoria" element={<PlaceholderPage title="Auditoría" description="Consulta futura de trazabilidad y auditoría." />} />
        <Route path="/respaldos" element={<PlaceholderPage title="Respaldos" description="Gestión futura de respaldos." />} />
      </Route>
      <Route path="*" element={<Navigate to="/inicio" replace />} />
    </Routes>
  );
}
