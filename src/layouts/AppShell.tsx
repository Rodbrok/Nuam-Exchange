import { useMemo, useState } from 'react';
import { Outlet } from 'react-router-dom';
import { mockUsers } from '../mocks/users';
import type { UserRole } from '../types';
import { Breadcrumbs } from './Breadcrumbs';
import { Header } from './Header';
import { Sidebar } from './Sidebar';

export function AppShell() {
  const [role, setRole] = useState<UserRole>('Administrador');
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const user = useMemo(() => mockUsers.find((item) => item.rol === role) ?? mockUsers[0], [role]);
  return <div className="app-shell"><Sidebar role={role} collapsed={collapsed} mobileOpen={mobileOpen} onClose={() => setMobileOpen(false)} onToggle={() => setCollapsed((value) => !value)} /><div className="app-shell__content"><Header user={user} role={role} onRoleChange={setRole} onMenu={() => setMobileOpen(true)} /><main><Breadcrumbs /><Outlet /></main></div>{mobileOpen && <button className="scrim" aria-label="Cerrar menú" onClick={() => setMobileOpen(false)} />}</div>;
}
