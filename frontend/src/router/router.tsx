import { createBrowserRouter, Navigate } from 'react-router-dom';
import { LoginPage } from '../pages/LoginPage';
import { DistributorSelectionPage } from '../pages/DistributorSelectionPage';
import { MainLayout } from '../components/MainLayout';
import { SalesDashboardPage } from '../features/dashboard/SalesDashboardPage';
import { ItemMasterPage } from '../features/master/ItemMasterPage';
import { SalesOrderPage } from '../features/transactions/SalesOrderPage';
import { PlaceholderPage } from '../pages/PlaceholderPage';

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/select-distributor', element: <DistributorSelectionPage /> },
  {
    path: '/',
    element: <MainLayout />,
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: 'dashboard', element: <SalesDashboardPage /> },
      { path: 'master/items', element: <ItemMasterPage /> },
      { path: 'transactions/sales-orders', element: <SalesOrderPage /> },
      { path: 'reports', element: <PlaceholderPage title="Reports" /> },
      { path: 'sync', element: <PlaceholderPage title="Synchronization" /> },
      { path: 'configuration', element: <PlaceholderPage title="Configuration" /> },
      { path: 'security', element: <PlaceholderPage title="Security" /> },
      { path: 'tools', element: <PlaceholderPage title="Tools" /> },
      { path: 'claims', element: <PlaceholderPage title="Claim Management" /> }
    ]
  }
]);
