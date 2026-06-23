import { Link } from 'react-router-dom';

export function LoginPage() {
  return (
    <main className="login-page">
      <section className="login-card" aria-labelledby="login-title">
        <p className="eyebrow">Nuam Exchange</p>
        <h1 id="login-title">Sistema de Gestión Tributaria</h1>
        <p>Pantalla visual de acceso. La autenticación real se implementará en una etapa posterior.</p>
        <form className="login-form">
          <label>
            Usuario
            <input type="text" placeholder="usuario@nuam.local" disabled />
          </label>
          <label>
            Contraseña
            <input type="password" placeholder="••••••••" disabled />
          </label>
          <Link className="primary-link" to="/inicio">Ingresar visualmente</Link>
        </form>
      </section>
    </main>
  );
}
