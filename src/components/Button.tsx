import type { ButtonHTMLAttributes, ReactNode } from 'react';

type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'ghost';
interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> { children: ReactNode; variant?: ButtonVariant; isLoading?: boolean }
export function Button({ children, variant = 'secondary', isLoading = false, disabled, ...props }: ButtonProps) {
  return <button className={`button button--${variant}`} disabled={disabled || isLoading} {...props}>{isLoading ? 'Procesando...' : children}</button>;
}
