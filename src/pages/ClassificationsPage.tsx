import { Button } from '../components/Button';
import { DataTableShell } from '../components/DataTableShell';
import { FormField } from '../components/FormField';
import { PageHeader } from '../components/PageHeader';
import { Panel } from '../components/Panel';
import { StatusBadge } from '../components/StatusBadge';
import { mockClassifications } from '../mocks/classifications';

export function ClassificationsPage() {
  return <><PageHeader title="Calificaciones Tributarias" description="Base visual inicial para consulta y mantenimiento posterior de calificaciones." actions={<Button variant="primary">Ingresar</Button>} />
    <Panel title="Filtros de búsqueda"><div className="filters"><FormField label="Mercado" options={['Todos', 'Local', 'Extranjero']} /><FormField label="Origen" options={['Todos', 'Manual', 'Carga masiva']} /><FormField label="Ejercicio" options={['Todos', '2026', '2025']} /><FormField label="Estado" options={['Todos', 'Validado', 'Pendiente', 'Observado']} /><div className="filter-actions"><Button variant="primary">Buscar</Button><Button>Limpiar</Button></div></div></Panel>
    <Panel title="Resultados" actions={<div className="toolbar"><Button>Copiar</Button><Button>Carga X Factor</Button><Button>Carga X Monto</Button></div>}><DataTableShell label="Tabla de calificaciones tributarias"><table><thead><tr><th>Ejercicio</th><th>Instrumento</th><th>Fecha de pago</th><th>Descripción</th><th>Secuencia de evento</th><th>Factor de actualización</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>{mockClassifications.map((item) => <tr key={item.id}><td>{item.ejercicio}</td><td>{item.instrumento}</td><td>{item.fechaPago}</td><td>{item.descripcion}</td><td>{item.secuenciaEvento}</td><td>{item.factorActualizacion.toFixed(4)}</td><td><StatusBadge status={item.estado} /></td><td className="row-actions"><Button>Modificar</Button><Button variant="danger">Eliminar</Button><Button>Copiar</Button><Button>Ver</Button></td></tr>)}</tbody></table></DataTableShell></Panel></>;
}
