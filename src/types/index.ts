export type UserRole = 'Administrador' | 'Analista Tributario' | 'Supervisor';

export interface MockUser { id: string; nombre: string; rol: UserRole; area: string }
export type ClassificationStatus = 'Validado' | 'Pendiente' | 'Observado' | 'Rechazado';
export interface TaxClassification { id: string; ejercicio: number; instrumento: string; fechaPago: string; descripcion: string; secuenciaEvento: string; factorActualizacion: number; estado: ClassificationStatus }
export interface NavItem { label: string; path: string; roles: UserRole[]; children?: NavItem[] }
