import { useEffect, useState } from 'react';
import { api } from '../../services/api';
import { useAppSelector } from '../../app/hooks';

export function ClaimsPage() {
  const distributor = useAppSelector((s) => s.distributor.selected);
  const [rows, setRows] = useState<any[]>([]);
  const [form, setForm] = useState({ claimType: 'Damage Claim', amount: 0, reason: '', docs: '' });
  const load = async () => { if (!distributor) return; const { data } = await api.get('/claims', { params: { distributorId: distributor.id } }); setRows(data.items || []); };
  useEffect(() => { load(); }, [distributor?.id]);
  const create = async () => { if (!distributor) return; await api.post('/claims', { distributorId: distributor.id, claimType: form.claimType, amount: form.amount, reason: form.reason, documents: form.docs.split(',').filter(Boolean) }); await load(); };
  return <div className="bg-white p-4 rounded shadow space-y-2"><h2 className="font-semibold">Claim Management</h2><select className="border p-2" onChange={(e) => setForm({ ...form, claimType: e.target.value })}><option>Damage Claim</option><option>Sample Claim</option><option>Rate Difference</option><option>Secondary Scheme Claim</option></select><input className="border p-2 w-full" type="number" placeholder="Amount" onChange={(e) => setForm({ ...form, amount: Number(e.target.value) })} /><input className="border p-2 w-full" placeholder="Reason" onChange={(e) => setForm({ ...form, reason: e.target.value })} /><input className="border p-2 w-full" placeholder="Documents (comma separated names)" onChange={(e) => setForm({ ...form, docs: e.target.value })} /><button className="bg-primary text-white px-3 py-2 rounded" onClick={create}>Create Claim</button>{rows.map((r) => <div key={r.id} className="border-b py-1">{r.claimType} - {r.amount} - {r.status}</div>)}</div>;
}
