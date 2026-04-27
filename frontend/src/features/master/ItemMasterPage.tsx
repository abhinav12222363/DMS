import { useEffect, useState } from 'react';
import { api } from '../../services/api';

interface Item { id: string; itemCode: string; name: string; unit: string; group: string }

export function ItemMasterPage() {
  const [items, setItems] = useState<Item[]>([]);
  const [search, setSearch] = useState('');

  const load = async () => {
    const { data } = await api.get('/master/items', { params: { search, pageNumber: 1, pageSize: 20 } });
    setItems(data.items);
  };

  useEffect(() => { void load(); }, []);

  return (
    <div className="bg-white rounded-lg shadow-sm p-4">
      <div className="flex justify-between mb-4">
        <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Search item" className="border rounded px-3 py-2" />
        <button onClick={load} className="bg-primary text-white px-4 py-2 rounded">Search</button>
      </div>
      <table className="w-full text-sm">
        <thead><tr className="text-left border-b"><th>Code</th><th>Name</th><th>Unit</th><th>Group</th></tr></thead>
        <tbody>{items.map((item) => <tr key={item.id} className="border-b"><td>{item.itemCode}</td><td>{item.name}</td><td>{item.unit}</td><td>{item.group}</td></tr>)}</tbody>
      </table>
    </div>
  );
}
