import type { MockUser, UserRole } from '../types';
export function Header({ user, role, onRoleChange, onMenu }: { user: MockUser; role: UserRole; onRoleChange: (role: UserRole) => void; onMenu: () => void }) {
  return <header className="topbar"><button className="mobile-menu" onClick={onMenu} aria-label="Abrir menú">☰</button><div><strong>Nuam Exchange</strong><span>Sistema de Gestión Tributaria</span></div><div className="topbar__user"><span>{user.nombre}</span><span>{role}</span><label>Rol demo<select value={role} onChange={(event) => onRoleChange(event.target.value as UserRole)}><option>Administrador</option><option>Analista Tributario</option><option>Supervisor</option></select></label></div></header>;
}
