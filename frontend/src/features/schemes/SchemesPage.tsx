import { useEffect, useState } from 'react';
import { api } from '../../services/api';

export function SchemesPage() {
  const [rows, setRows] = useState<any[]>([]);
  const [name, setName] = useState('');
  const load = async () => setRows((await api.get('/schemes')).data);
  useEffect(() => { load(); }, []);
  const create = async () => { await api.post('/schemes', { name, schemeType: 'OnSpot', applyOn: 'Item Group', calculationMode: 'Exclusive', validFrom: new Date().toISOString().slice(0, 10), validTo: new Date(Date.now() + 86400000 * 30).toISOString().slice(0, 10), itemGroup: 'GENERAL', slabs: [{ minQty: 1, minValue: 1, discountPercent: 5, freeQty: 0, cashbackAmount: 0, points: 0 }] }); load(); };
  return <div className="bg-white p-4 rounded shadow space-y-2"><h2 className="font-semibold">Schemes</h2><input className="border p-2 w-full" placeholder="Scheme Name" onChange={(e) => setName(e.target.value)} /><button className="bg-primary text-white px-3 py-2 rounded" onClick={create}>Create Scheme</button>{rows.map((r) => <div key={r.id} className="flex justify-between border-b py-1"><span>{r.name} ({r.status})</span><button onClick={() => api.post(`/schemes/${r.id}/approve`).then(load)} className="text-primary">Approve</button></div>)}</div>;
}
