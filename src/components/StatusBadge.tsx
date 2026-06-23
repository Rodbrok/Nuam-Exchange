import type { ClassificationStatus } from '../types/classification';

interface StatusBadgeProps { status: ClassificationStatus; }

export function StatusBadge({ status }: StatusBadgeProps) {
  return <span className={`status-badge status-${status.toLowerCase()}`}>{status}</span>;
}
