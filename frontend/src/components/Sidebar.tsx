import { BarChart3, Boxes, FileText, RefreshCcw, Settings, Shield, Wrench, ReceiptText } from 'lucide-react';
import { NavLink } from 'react-router-dom';

const menus = [
  { to: '/dashboard', icon: BarChart3, label: 'Dashboard and Chart' },
  { to: '/master/items', icon: Boxes, label: 'Master' },
  { to: '/transactions/sales-orders', icon: ReceiptText, label: 'Transactions' },
  { to: '/reports', icon: FileText, label: 'Reports' },
  { to: '/sync', icon: RefreshCcw, label: 'Synchronization' },
  { to: '/configuration', icon: Settings, label: 'Configuration' },
  { to: '/security', icon: Shield, label: 'Security' },
  { to: '/tools', icon: Wrench, label: 'Tools' }
];

export function Sidebar() {
  return (
    <aside className="w-20 hover:w-64 transition-all duration-300 bg-primary text-white min-h-screen p-3 overflow-hidden">
      <div className="text-sm font-semibold mb-4">DMS</div>
      <nav className="space-y-2">
        {menus.map((menu) => (
          <NavLink key={menu.to} to={menu.to} className="group flex items-center gap-3 rounded-md px-2 py-2 hover:bg-white/20">
            <menu.icon className="h-5 w-5" />
            <span className="opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">{menu.label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
