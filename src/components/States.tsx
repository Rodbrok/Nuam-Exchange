export function EmptyState({ title, message }: { title: string; message: string }) { return <div className="state"><strong>{title}</strong><span>{message}</span></div>; }
export function LoadingState({ message = 'Cargando información...' }: { message?: string }) { return <div className="state" aria-live="polite">{message}</div>; }
export function ErrorState({ message }: { message: string }) { return <div className="state state--error" role="alert">{message}</div>; }
