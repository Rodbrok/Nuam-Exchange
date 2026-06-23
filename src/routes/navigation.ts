import type { NavigationItem } from '../types/navigation';

export const navigationItems: NavigationItem[] = [
  { label: 'Inicio', path: '/inicio' },
  { label: 'Calificaciones Tributarias', path: '/calificaciones' },
  { label: 'Carga X Factor', path: '/cargas/x-factor' },
  { label: 'Carga X Monto', path: '/cargas/x-monto' },
  { label: 'Plantillas de carga', path: '/plantillas-carga' },
  { label: 'Reportes', path: '/reportes' },
  { label: 'Usuarios', path: '/administracion/usuarios' },
  { label: 'Roles y Permisos', path: '/administracion/roles-permisos' },
  { label: 'Auditoría', path: '/auditoria' },
  { label: 'Respaldos', path: '/respaldos' },
];
