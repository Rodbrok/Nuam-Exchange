import { useParams } from 'react-router-dom';

export function EditarCalificacionPage() {
  const { id } = useParams();

  return (
    <section className="card">
      <h1>Editar calificación</h1>
      <p>Ruta parametrizada activa para la calificación {id}.</p>
      <p>No se implementa edición real en esta etapa.</p>
    </section>
  );
}
