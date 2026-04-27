import { Search } from 'lucide-react';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { logout } from '../features/auth/authSlice';

export function Header() {
  const username = useAppSelector((s) => s.auth.username ?? 'User');
  const dispatch = useAppDispatch();

  return (
    <header className="h-16 bg-white border-b px-6 flex items-center justify-between">
      <div className="mx-auto w-1/2 relative">
        <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
        <input className="w-full border rounded-full py-2 pl-9 pr-3" placeholder="Search menu" />
      </div>
      <div className="relative group">
        <button className="text-sm font-medium">{username}</button>
        <div className="hidden group-hover:block absolute right-0 top-7 bg-white border rounded-md w-56 p-2 shadow">
          <button className="block w-full text-left px-2 py-1 hover:bg-slate-100">Change Distributor</button>
          <button className="block w-full text-left px-2 py-1 hover:bg-slate-100">Profile</button>
          <button className="block w-full text-left px-2 py-1 hover:bg-slate-100" onClick={() => dispatch(logout())}>Logout</button>
        </div>
      </div>
    </header>
  );
}
