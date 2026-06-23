import type { ClassificationStatus } from '../types';
const tone: Record<ClassificationStatus, string> = { Validado: 'success', Pendiente: 'warning', Observado: 'info', Rechazado: 'danger' };
export function StatusBadge({ status }: { status: ClassificationStatus }) { return <span className={`status status--${tone[status]}`}>{status}</span>; }
