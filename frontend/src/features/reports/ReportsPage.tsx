import { useEffect, useState } from 'react';
import { api } from '../../services/api';
import { useAppSelector } from '../../app/hooks';

export function ReportsPage() {
  const distributor = useAppSelector((s) => s.distributor.selected);
  const [data, setData] = useState<any>({});
  useEffect(() => { if (!distributor) return; Promise.all([api.get('/reports/orders', { params: { distributorId: distributor.id } }), api.get('/reports/stock', { params: { distributorId: distributor.id } }), api.get('/reports/claims', { params: { distributorId: distributor.id } }), api.get('/reports/schemes')]).then(([orders, stock, claims, schemes]) => setData({ orders: orders.data, stock: stock.data, claims: claims.data, schemes: schemes.data })); }, [distributor?.id]);
  return <div className="grid grid-cols-2 gap-4">{['orders', 'stock', 'claims', 'schemes'].map((k) => <div key={k} className="bg-white p-4 rounded shadow"><h3 className="font-semibold capitalize">{k} report</h3><pre className="text-xs overflow-auto">{JSON.stringify(data[k], null, 2)}</pre></div>)}</div>;
}
