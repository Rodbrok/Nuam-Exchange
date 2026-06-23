type SimplePageProps = {
  title: string;
};

export function SimplePage({ title }: SimplePageProps) {
  return (
    <section className="card">
      <h1>{title}</h1>
      <p>Vista estática para revisión visual de la navegación inicial.</p>
    </section>
  );
}
