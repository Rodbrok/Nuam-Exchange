import { Link, useLocation } from 'react-router-dom';
export function Breadcrumbs() { const parts = useLocation().pathname.split('/').filter(Boolean); return <nav className="breadcrumbs" aria-label="Ruta de navegación"><Link to="/inicio">Inicio</Link>{parts.map((part, index) => <span key={`${part}-${index}`}> / {part.replace('-', ' ')}</span>)}</nav>; }
