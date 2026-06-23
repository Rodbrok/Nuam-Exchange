import { Link } from 'react-router-dom';
import { PageHeader } from '../components/PageHeader';

export function InicioPage() {
  return (
    <section className="content-card">
      <PageHeader
        title="Inicio"
        description="Base inicial del frontend administrativo para la gestión tributaria interna."
      />
      <div className="dashboard-grid">
        <article>
          <h2>Calificaciones</h2>
          <p>Acceso al futuro módulo de calificaciones tributarias.</p>
          <Link to="/calificaciones">Ver módulo</Link>
        </article>
        <article>
          <h2>Cargas</h2>
          <p>Rutas preparadas para X Factor, X Monto y plantillas de carga.</p>
          <Link to="/cargas/x-factor">Ir a cargas</Link>
        </article>
        <article>
          <h2>Administración</h2>
          <p>Espacios reservados para usuarios, roles y auditoría.</p>
          <Link to="/administracion/usuarios">Ver administración</Link>
        </article>
      </div>
    </section>
  );
}
