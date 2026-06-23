import type { NavItem } from '../types';

const allRoles = ['Administrador', 'Analista Tributario', 'Supervisor'] as const;

export const navigationItems: NavItem[] = [
  { label: 'Inicio', path: '/inicio', roles: [...allRoles] },
  { label: 'Calificaciones Tributarias', path: '/calificaciones', roles: [...allRoles] },
  { label: 'Cargas masivas', path: '/cargas/x-factor', roles: [...allRoles], children: [
    { label: 'Carga X Factor', path: '/cargas/x-factor', roles: ['Administrador', 'Analista Tributario', 'Supervisor'] },
    { label: 'Carga X Monto', path: '/cargas/x-monto', roles: ['Administrador', 'Analista Tributario'] },
    { label: 'Plantillas de carga', path: '/plantillas-carga', roles: [...allRoles] },
  ] },
  { label: 'Reportes', path: '/reportes', roles: [...allRoles] },
  { label: 'Administración', path: '/administracion/usuarios', roles: ['Administrador'], children: [
    { label: 'Usuarios', path: '/administracion/usuarios', roles: ['Administrador'] },
    { label: 'Roles y Permisos', path: '/administracion/roles-permisos', roles: ['Administrador'] },
  ] },
  { label: 'Auditoría', path: '/auditoria', roles: ['Administrador'] },
  { label: 'Respaldos', path: '/respaldos', roles: ['Administrador'] },
];
