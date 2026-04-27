import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { fetchDistributors, selectDistributor } from '../features/distributor/distributorSlice';

export function DistributorSelectionPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const distributors = useAppSelector((s) => s.distributor.available);

  useEffect(() => { void dispatch(fetchDistributors()); }, [dispatch]);

  return (
    <div className="min-h-screen grid place-items-center bg-slate-100">
      <div className="bg-white rounded-xl p-6 w-full max-w-xl shadow">
        <h2 className="text-xl font-semibold mb-4">Select Distributor</h2>
        <div className="space-y-2">
          {distributors.map((d) => (
            <button key={d.id} className="w-full text-left border rounded p-3 hover:border-primary" onClick={() => { dispatch(selectDistributor(d)); navigate('/dashboard'); }}>
              <p className="font-medium">{d.name}</p>
              <p className="text-xs text-slate-500">{d.code} • {d.zone}</p>
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}
