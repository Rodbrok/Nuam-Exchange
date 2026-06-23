import { useParams } from 'react-router-dom';
import { PageHeader } from './PageHeader';

interface PlaceholderPageProps {
  title: string;
  description: string;
}

export function PlaceholderPage({ title, description }: PlaceholderPageProps) {
  const { id } = useParams();

  return (
    <section className="content-card">
      <PageHeader title={title} description={description} />
      <div className="placeholder-panel" aria-label="Contenido pendiente">
        <h2>Módulo pendiente de implementación</h2>
        <p>
          Esta pantalla define la ubicación y navegación del módulo. La lógica funcional se incorporará en próximos prompts.
        </p>
        {id ? <p className="metadata">Identificador recibido: {id}</p> : null}
      </div>
    </section>
  );
}
