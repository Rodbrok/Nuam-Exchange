import { useEffect, useMemo, useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSession } from '../app/session/useSession';
import { Button } from '../components/Button';
import { DataTableShell } from '../components/DataTableShell';
import { EmptyState, ErrorState, LoadingState } from '../components/ViewStates';
import { FormField } from '../components/FormField';
import { InlineMessage } from '../components/InlineMessage';
import { PageHeader } from '../components/PageHeader';
import { Pagination } from '../components/Pagination';
import { mockClassifications } from '../mocks/classifications';
import type { Classification, ClassificationFilters, ClassificationSortKey, PaginationState, SortState } from '../types/classification';

const emptyFilters: ClassificationFilters = { mercado: 'Todos', origen: 'Todos', ejercicio: 'Todos', estado: 'Todos', texto: '' };
type DemoState = 'Normal' | 'Cargando' | 'Error';

function parseDate(value: string) { const [day, month, year] = value.split('-').map(Number); return new Date(year, month - 1, day).getTime(); }
function filterRecords(records: Classification[], filters: ClassificationFilters) {
  const text = filters.texto.trim().toLowerCase();
  return records.filter((record) =>
    (filters.mercado === 'Todos' || record.mercado === filters.mercado) &&
    (filters.origen === 'Todos' || record.origen === filters.origen) &&
    (filters.ejercicio === 'Todos' || String(record.ejercicio) === filters.ejercicio) &&
    (filters.estado === 'Todos' || record.estado === filters.estado) &&
    (!text || [record.instrumento, record.descripcion, record.secuenciaEvento].some((field) => field.toLowerCase().includes(text)))
  );
}
function sortRecords(records: Classification[], sort: SortState) {
  return [...records].sort((a, b) => {
    const multiplier = sort.direction === 'asc' ? 1 : -1;
    if (sort.key === 'fechaPago') return (parseDate(a.fechaPago) - parseDate(b.fechaPago)) * multiplier;
    const left = a[sort.key];
    const right = b[sort.key];
    return (typeof left === 'number' && typeof right === 'number' ? left - right : String(left).localeCompare(String(right), 'es-CL')) * multiplier;
  });
}

export function ClassificationsPage() {
  const navigate = useNavigate();
  const { user } = useSession();
  const [draftFilters, setDraftFilters] = useState<ClassificationFilters>(emptyFilters);
  const [appliedFilters, setAppliedFilters] = useState<ClassificationFilters>(emptyFilters);
  const [sort, setSort] = useState<SortState>({ key: 'ejercicio', direction: 'asc' });
  const [pagination, setPagination] = useState<PaginationState>({ page: 1, pageSize: 10 });
  const [selected, setSelected] = useState<Classification | null>(null);
  const [message, setMessage] = useState('Seleccione un registro para habilitar acciones contextuales.');
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const [demoState, setDemoState] = useState<DemoState>('Normal');

  useEffect(() => { const timer = window.setTimeout(() => setIsInitialLoading(false), 350); return () => window.clearTimeout(timer); }, []);

  const filtered = useMemo(() => filterRecords(mockClassifications, appliedFilters), [appliedFilters]);
  const sorted = useMemo(() => sortRecords(filtered, sort), [filtered, sort]);
  const totalPages = Math.max(1, Math.ceil(sorted.length / pagination.pageSize));
  const visible = sorted.slice((pagination.page - 1) * pagination.pageSize, pagination.page * pagination.pageSize);

  useEffect(() => { if (pagination.page > totalPages) setPagination((current) => ({ ...current, page: totalPages })); }, [pagination.page, totalPages]);

  const selectOptions = (key: keyof Classification) => ['Todos', ...Array.from(new Set(mockClassifications.map((record) => String(record[key]))))];
  const canEdit = user?.rol === 'Administrador' || user?.rol === 'Analista Tributario';
  const canEnter = canEdit;
  const canMassLoad = Boolean(user);

  function applyFilters(event: FormEvent) { event.preventDefault(); setAppliedFilters(draftFilters); setPagination((current) => ({ ...current, page: 1 })); setSelected(null); setMessage('Filtros aplicados en memoria.'); }
  function clearFilters() { setDraftFilters(emptyFilters); setAppliedFilters(emptyFilters); setPagination((current) => ({ ...current, page: 1 })); setSelected(null); setMessage('Filtros limpiados.'); }
  function handleSort(key: ClassificationSortKey) { setSort((current) => ({ key, direction: current.key === key && current.direction === 'asc' ? 'desc' : 'asc' })); }
  function requireSelection(action: string, callback: (record: Classification) => void) { if (!selected) { setMessage(`${action} requiere seleccionar un registro.`); return; } callback(selected); }

  const isLoading = isInitialLoading || demoState === 'Cargando';
  return <section className="content-card classifications-page">
    <PageHeader title="Calificaciones Tributarias" description="Consulta y administración visual de registros tributarios." />
    <div className="demo-panel"><FormField id="demo-state" label="Estado de demostración"><select id="demo-state" value={demoState} onChange={(event) => setDemoState(event.target.value as DemoState)}><option>Normal</option><option>Cargando</option><option>Error</option></select></FormField><span>Control temporal para simular estados hasta integrar la API.</span></div>
    <form className="filters-panel" onSubmit={applyFilters}>
      <FormField id="mercado" label="Mercado"><select id="mercado" value={draftFilters.mercado} onChange={(e) => setDraftFilters({ ...draftFilters, mercado: e.target.value })}>{selectOptions('mercado').map((item) => <option key={item}>{item}</option>)}</select></FormField>
      <FormField id="origen" label="Origen"><select id="origen" value={draftFilters.origen} onChange={(e) => setDraftFilters({ ...draftFilters, origen: e.target.value })}>{selectOptions('origen').map((item) => <option key={item}>{item}</option>)}</select></FormField>
      <FormField id="ejercicio" label="Ejercicio"><select id="ejercicio" value={draftFilters.ejercicio} onChange={(e) => setDraftFilters({ ...draftFilters, ejercicio: e.target.value })}>{selectOptions('ejercicio').map((item) => <option key={item}>{item}</option>)}</select></FormField>
      <FormField id="estado" label="Estado"><select id="estado" value={draftFilters.estado} onChange={(e) => setDraftFilters({ ...draftFilters, estado: e.target.value })}>{selectOptions('estado').map((item) => <option key={item}>{item}</option>)}</select></FormField>
      <FormField id="texto" label="Texto libre"><input id="texto" value={draftFilters.texto} onChange={(e) => setDraftFilters({ ...draftFilters, texto: e.target.value })} placeholder="Instrumento, descripción o secuencia" /></FormField>
      <div className="filter-actions"><Button variant="primary" type="submit">Buscar</Button><Button type="button" onClick={clearFilters}>Limpiar</Button></div>
    </form>
    <div className="actions-bar" aria-label="Acciones principales">
      {canEnter ? <Button variant="primary" onClick={() => navigate('/calificaciones/nueva')}>Ingresar</Button> : null}
      {canEdit ? <Button disabled={!selected} onClick={() => requireSelection('Modificar', (record) => navigate(`/calificaciones/${record.id}/editar`))}>Modificar</Button> : null}
      {canEdit ? <Button disabled={!selected} onClick={() => requireSelection('Eliminar', () => setMessage('La eliminación real se implementará posteriormente.'))}>Eliminar</Button> : null}
      {canEdit ? <Button disabled={!selected} onClick={() => requireSelection('Copiar', () => setMessage('La copia se implementará en el siguiente prompt.'))}>Copiar</Button> : null}
      {canMassLoad ? <Button onClick={() => navigate('/cargas/x-factor')}>Carga X Factor</Button> : null}
      {canMassLoad ? <Button onClick={() => navigate('/cargas/x-monto')}>Carga X Monto</Button> : null}
      <Button disabled={!selected} onClick={() => requireSelection('Opciones', (record) => setMessage(`Registro ${record.id}: ${record.instrumento}, ${record.estado}, monto ${record.monto.toLocaleString('es-CL')}.`))}>Opciones</Button>
    </div>
    <InlineMessage message={`${message} Resultados encontrados: ${filtered.length}.`} />
    {demoState === 'Error' ? <ErrorState title="Error de demostración" description="Estado temporal para validar la vista de error antes de integrar la API." /> : isLoading ? <LoadingState /> : filtered.length === 0 ? <EmptyState title="Sin resultados" description="No existen calificaciones para los filtros aplicados." actionLabel="Limpiar filtros" onAction={clearFilters} /> : <><DataTableShell records={visible} selectedId={selected?.id ?? null} sort={sort} onSort={handleSort} onSelect={(record) => { setSelected(record); setMessage(`Registro seleccionado: ${record.id}.`); }} /><Pagination pagination={pagination} totalItems={sorted.length} onPageChange={(page) => setPagination((current) => ({ ...current, page }))} onPageSizeChange={(pageSize) => setPagination({ page: 1, pageSize })} /></>}
  </section>;
}
