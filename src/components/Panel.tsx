import type { ReactNode } from 'react';
export function Panel({ title, children, actions }: { title?: string; children: ReactNode; actions?: ReactNode }) {
  return <section className="panel">{(title || actions) && <div className="panel__header">{title && <h2>{title}</h2>}<div>{actions}</div></div>}<div className="panel__body">{children}</div></section>;
}
