import type { TaxClassification } from '../types';

export const mockClassifications: TaxClassification[] = [
  { id: '1', ejercicio: 2026, instrumento: 'ACC-NUAM-001', fechaPago: '15-04-2026', descripcion: 'Dividendo ficticio mercado local', secuenciaEvento: 'EVT-00045', factorActualizacion: 1.0342, estado: 'Validado' },
  { id: '2', ejercicio: 2026, instrumento: 'BONO-CORP-2040', fechaPago: '28-05-2026', descripcion: 'Interés bono corporativo ficticio', secuenciaEvento: 'EVT-00068', factorActualizacion: 1.0189, estado: 'Pendiente' },
  { id: '3', ejercicio: 2025, instrumento: 'ETF-REG-010', fechaPago: '12-12-2025', descripcion: 'Distribución de fondo ficticia', secuenciaEvento: 'EVT-00031', factorActualizacion: 1.0521, estado: 'Observado' },
];
