import type { UploadConfig } from './uploadConfigs';
import type { UploadValidationError } from '../../types/upload';
const safe=(v:string)=>{ const pref=/^[=+\-@]/.test(v)?`'${v}`:v; return /[";,\n\r]/.test(pref)?`"${pref.replace(/"/g,'""')}"`:pref; };
function downloadCsv(name:string, rows:string[][], bom=false){ const csv=(bom?'\uFEFF':'')+rows.map((r)=>r.map(safe).join(';')).join('\n'); const url=URL.createObjectURL(new Blob([csv],{type:'text/csv;charset=utf-8'})); const a=document.createElement('a'); a.href=url; a.download=name; a.click(); URL.revokeObjectURL(url); }
export function downloadTemplate(config:UploadConfig){ downloadCsv(`plantilla-${config.type}.csv`, [config.columns.map((c)=>c.key), config.examples[0]], true); }
export function downloadErrors(type:string, errors:UploadValidationError[]){ downloadCsv(`errores-${type}.csv`, [['fila','columna','codigo','mensaje'], ...errors.map((e)=>[String(e.rowNumber),e.column,e.code,e.message])]); }
export const formatBytes=(bytes:number)=> bytes < 1024 ? `${bytes} B` : bytes < 1024*1024 ? `${(bytes/1024).toFixed(1)} KB` : `${(bytes/1024/1024).toFixed(2)} MB`;
