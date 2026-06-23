interface HeaderProps {
  isCollapsed: boolean;
  onToggleSidebar: () => void;
  onToggleMobile: () => void;
}

export function Header({ isCollapsed, onToggleSidebar, onToggleMobile }: HeaderProps) {
  return (
    <header className="app-header">
      <div className="header-actions">
        <button className="icon-button desktop-only" type="button" onClick={onToggleSidebar} aria-pressed={isCollapsed}>
          {isCollapsed ? 'Expandir menú' : 'Colapsar menú'}
        </button>
        <button className="icon-button mobile-only" type="button" onClick={onToggleMobile}>
          Abrir menú
        </button>
        <div>
          <strong>Nuam Exchange</strong>
          <span>Sistema de Gestión Tributaria</span>
        </div>
      </div>
      <div className="user-area">
        <span>Usuario Administrador</span>
        <button type="button" className="secondary-button">Cerrar sesión</button>
      </div>
    </header>
  );
}
