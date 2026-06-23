import { useNavigate } from 'react-router-dom';

export function LoginPage() {
  const navigate = useNavigate();

  return (
    <main className="login-page">
      <section className="login-card">
        <h1>Nuam Exchange</h1>
        <p>Acceso visual sin autenticación real.</p>
        <button type="button" onClick={() => navigate('/inicio')}>
          Ingresar
        </button>
      </section>
    </main>
  );
}
