import { NavLink } from 'react-router-dom';
import { navigationItems } from '../routes/navigation';
import type { UserRole } from '../types';

export function Sidebar({ role, collapsed, mobileOpen, onClose, onToggle }: { role: UserRole; collapsed: boolean; mobileOpen: boolean; onClose: () => void; onToggle: () => void }) {
  const visible = navigationItems.filter((item) => item.roles.includes(role));
  return <aside className={`sidebar ${collapsed ? 'is-collapsed' : ''} ${mobileOpen ? 'is-open' : ''}`}>
    <button className="sidebar__toggle" onClick={onToggle} aria-label="Contraer o expandir menú">☰</button>
    <nav aria-label="Navegación principal">{visible.map((item) => <div className="nav-group" key={item.label}><NavLink to={item.path} onClick={onClose}>{item.label}</NavLink>{item.children?.filter((child) => child.roles.includes(role)).map((child) => <NavLink className="nav-child" to={child.path} key={child.path} onClick={onClose}>{child.label}</NavLink>)}</div>)}</nav>
    <NavLink className="logout-link" to="/login" onClick={onClose}>Cerrar sesión</NavLink>
  </aside>;
}
