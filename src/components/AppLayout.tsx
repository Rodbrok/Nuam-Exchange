import { FileBarChart, Home, Shield, UploadCloud } from 'lucide-react';
import { NavLink, Outlet } from 'react-router-dom';

type Role = 'Administrador' | 'Supervisor';

type NavigationItem = {
  label: string;
  path: string;
  icon: JSX.Element;
  roles: Role[];
};

const currentRole: Role = 'Supervisor';

const navigationItems: NavigationItem[] = [
  {
    label: 'Inicio',
    path: '/inicio',
    icon: <Home size={18} />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Carga X Factor',
    path: '/carga-x-factor',
    icon: <UploadCloud size={18} />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Carga X Monto',
    path: '/carga-x-monto',
    icon: <UploadCloud size={18} />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Plantillas de carga',
    path: '/plantillas-carga',
    icon: <FileBarChart size={18} />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Reportes',
    path: '/reportes',
    icon: <FileBarChart size={18} />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Administración',
    path: '/administracion',
    icon: <Shield size={18} />,
    roles: ['Administrador'],
  },
  {
    label: 'Usuarios',
    path: '/usuarios',
    icon: <Shield size={18} />,
    roles: ['Administrador'],
  },
  {
    label: 'Roles y permisos',
    path: '/roles-permisos',
    icon: <Shield size={18} />,
    roles: ['Administrador'],
  },
  {
    label: 'Respaldos',
    path: '/respaldos',
    icon: <Shield size={18} />,
    roles: ['Administrador'],
  },
  {
    label: 'Auditoría',
    path: '/auditoria',
    icon: <Shield size={18} />,
    roles: ['Administrador'],
  },
];

export function AppLayout() {
  const visibleItems = navigationItems.filter((item) => item.roles.includes(currentRole));

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <strong>Nuam Exchange</strong>
        <span className="role-label">Rol visual: {currentRole}</span>
        <nav>
          {visibleItems.map((item) => (
            <NavLink key={item.path} to={item.path} className="nav-link">
              {item.icon}
              {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
