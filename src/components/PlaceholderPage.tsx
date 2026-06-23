import { Panel } from './Panel';
import { PageHeader } from './PageHeader';
export function PlaceholderPage({ title, description }: { title: string; description: string }) { return <><PageHeader title={title} description={description} /><Panel title="Área de contenido"><p>Este módulo está preparado para desarrollo posterior. La estructura visual, navegación y contenedores ya están disponibles para incorporar la funcionalidad específica.</p></Panel></>; }
