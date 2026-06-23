import type { SelectHTMLAttributes } from 'react';
interface FormFieldProps extends SelectHTMLAttributes<HTMLSelectElement> { label: string; options: string[] }
export function FormField({ label, options, id, ...props }: FormFieldProps) { const fieldId = id ?? label.toLowerCase().replaceAll(' ', '-'); return <label className="form-field" htmlFor={fieldId}><span>{label}</span><select id={fieldId} {...props}>{options.map((option) => <option key={option}>{option}</option>)}</select></label>; }
