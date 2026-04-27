import { useMemo } from 'react';
import { Bar, BarChart, CartesianGrid, Line, LineChart, ResponsiveContainer, XAxis, YAxis } from 'recharts';

const trend = [
  { label: 'W1', sales: 12000 }, { label: 'W2', sales: 18000 }, { label: 'W3', sales: 16000 }, { label: 'W4', sales: 22000 }
];
const topItems = [
  { label: 'Item A', value: 90 }, { label: 'Item B', value: 80 }, { label: 'Item C', value: 60 }, { label: 'Item D', value: 50 }
];

export function SalesDashboardPage() {
  const kpis = useMemo(() => ([
    { label: 'Sales Value', value: '₹12.5M' },
    { label: 'Orders', value: '2,145' },
    { label: 'Avg Ticket', value: '₹5,829' },
    { label: 'Top 10 Share', value: '64%' }
  ]), []);

  return (
    <div className="space-y-6">
      <div className="flex gap-4">
        <input type="date" className="border rounded px-3 py-2" />
        <input type="date" className="border rounded px-3 py-2" />
      </div>
      <div className="grid md:grid-cols-4 gap-4">
        {kpis.map((kpi) => <div key={kpi.label} className="bg-white rounded-lg p-4 shadow-sm"><p className="text-sm text-slate-500">{kpi.label}</p><p className="text-2xl font-bold">{kpi.value}</p></div>)}
      </div>
      <div className="grid lg:grid-cols-2 gap-4">
        <div className="bg-white rounded-lg p-4 h-72"><h3 className="font-semibold mb-2">Sales Trend</h3><ResponsiveContainer width="100%" height="100%"><LineChart data={trend}><CartesianGrid strokeDasharray="3 3" /><XAxis dataKey="label" /><YAxis /><Line dataKey="sales" stroke="#0B5C89" /></LineChart></ResponsiveContainer></div>
        <div className="bg-white rounded-lg p-4 h-72"><h3 className="font-semibold mb-2">Top 10 Items</h3><ResponsiveContainer width="100%" height="100%"><BarChart data={topItems}><CartesianGrid strokeDasharray="3 3" /><XAxis dataKey="label" /><YAxis /><Bar dataKey="value" fill="#4DA8DA" /></BarChart></ResponsiveContainer></div>
      </div>
    </div>
  );
}
