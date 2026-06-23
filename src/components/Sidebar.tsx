import { NavLink } from 'react-router-dom';
import { useSession } from '../app/session/useSession';
import { getNavigationForRole } from '../routes/navigation';

interface SidebarProps {
  isCollapsed: boolean;
  isMobileOpen: boolean;
  onCloseMobile: () => void;
}

export function Sidebar({ isCollapsed, isMobileOpen, onCloseMobile }: SidebarProps) {
  const { user } = useSession();
  const authorizedItems = user ? getNavigationForRole(user.rol) : [];

  return (
    <aside className={`sidebar ${isCollapsed ? 'is-collapsed' : ''} ${isMobileOpen ? 'is-open' : ''}`}>
      <div className="sidebar-brand">
        <strong>Nuam Exchange</strong>
        <span>Sistema Tributario</span>
      </div>
      <nav aria-label="Menú principal">
        {authorizedItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) => `sidebar-link ${isActive ? 'is-active' : ''}`}
            onClick={onCloseMobile}
          >
            <span className="sidebar-marker" aria-hidden="true" />
            <span>{item.label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
