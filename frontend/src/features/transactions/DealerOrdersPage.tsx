import { useEffect, useState } from 'react';
import { api } from '../../services/api';
import { useAppSelector } from '../../app/hooks';

interface Item { id: string; name: string; basePrice: number; }

export function DealerOrdersPage() {
  const distributor = useAppSelector((s) => s.distributor.selected);
  const [items, setItems] = useState<Item[]>([]);
  const [rows, setRows] = useState([{ itemId: '', quantity: 0, rate: 0 }]);
  const [orders, setOrders] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    api.get('/master/items').then((r) => setItems(r.data.items ?? r.data));
  }, []);

  const loadOrders = async () => {
    if (!distributor) return;
    const { data } = await api.get('/dealer/orders', { params: { distributorId: distributor.id } });
    setOrders(data.items || []);
  };

  useEffect(() => { loadOrders(); }, [distributor?.id]);

  const createOrder = async (submitForApproval: boolean) => {
    if (!distributor) return;
    setLoading(true);
    await api.post('/dealer/orders', { distributorId: distributor.id, orderDate: new Date().toISOString().slice(0, 10), submitForApproval, items: rows });
    setLoading(false);
    setRows([{ itemId: '', quantity: 0, rate: 0 }]);
    loadOrders();
  };

  const copyPrevious = async (orderId: string) => {
    await api.post(`/dealer/orders/${orderId}/copy`, null, { params: { newDate: new Date().toISOString().slice(0, 10) } });
    loadOrders();
  };

  const bulkUpload = (text: string) => {
    const mapped = text.split('\n').filter(Boolean).map((x) => {
      const [itemId, quantity, rate] = x.split(',');
      return { itemId, quantity: Number(quantity), rate: Number(rate) };
    });
    if (mapped.length) setRows(mapped);
  };

  return <div className="space-y-4">
    <div className="bg-white p-4 rounded shadow space-y-2">
      <h2 className="font-semibold">Dealer Order Entry</h2>
      {rows.map((r, idx) => <div key={idx} className="grid grid-cols-3 gap-2">
        <select className="border p-2" value={r.itemId} onChange={(e) => setRows(rows.map((x, i) => i === idx ? { ...x, itemId: e.target.value } : x))}>
          <option value="">Select Item</option>
          {items.map((i) => <option key={i.id} value={i.id}>{i.name}</option>)}
        </select>
        <input type="number" className="border p-2" placeholder="Qty" onChange={(e) => setRows(rows.map((x, i) => i === idx ? { ...x, quantity: Number(e.target.value) } : x))} />
        <input type="number" className="border p-2" placeholder="Rate" onChange={(e) => setRows(rows.map((x, i) => i === idx ? { ...x, rate: Number(e.target.value) } : x))} />
      </div>)}
      <button className="border px-3 py-1" onClick={() => setRows([...rows, { itemId: '', quantity: 0, rate: 0 }])}>+ Add Item</button>
      <textarea className="border p-2 w-full" placeholder="Bulk upload: itemId,qty,rate" onBlur={(e) => bulkUpload(e.target.value)} />
      <div className="flex gap-2">
        <button className="bg-slate-700 text-white px-3 py-2 rounded" disabled={loading} onClick={() => createOrder(false)}>Save Draft</button>
        <button className="bg-primary text-white px-3 py-2 rounded" disabled={loading} onClick={() => createOrder(true)}>Submit Approval</button>
      </div>
    </div>
    <div className="bg-white p-4 rounded shadow">
      <h3 className="font-semibold">Recent Orders</h3>
      {orders.map((o) => <div key={o.id} className="flex justify-between border-b py-2"><span>{o.orderNumber} - {o.status}</span><button className="text-primary" onClick={() => copyPrevious(o.id)}>Copy</button></div>)}
    </div>
  </div>;
}
