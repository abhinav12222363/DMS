import { useState } from 'react';
import { api } from '../../services/api';
import { useAppSelector } from '../../app/hooks';

export function SalesOrderPage() {
  const distributor = useAppSelector((s) => s.distributor.selected);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({ orderNumber: '', orderDate: '', totalAmount: 0 });

  const submit = async () => {
    if (!distributor) return;
    setSaving(true);
    await api.post('/transactions/sales-orders', { ...form, distributorId: distributor.id });
    setSaving(false);
  };

  return (
    <div className="bg-white rounded-lg shadow-sm p-6 space-y-4 max-w-xl">
      <h2 className="text-xl font-semibold">Sales Order</h2>
      <input className="border rounded px-3 py-2 w-full" placeholder="Order Number" onChange={(e) => setForm({ ...form, orderNumber: e.target.value })} />
      <input type="date" className="border rounded px-3 py-2 w-full" onChange={(e) => setForm({ ...form, orderDate: e.target.value })} />
      <input type="number" className="border rounded px-3 py-2 w-full" placeholder="Total Amount" onChange={(e) => setForm({ ...form, totalAmount: Number(e.target.value) })} />
      <button disabled={saving} onClick={submit} className="bg-primary text-white px-4 py-2 rounded disabled:opacity-60">{saving ? 'Submitting...' : 'Submit'}</button>
    </div>
  );
}
