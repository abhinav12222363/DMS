import { createBrowserRouter, Navigate } from 'react-router-dom';
import { LoginPage } from '../pages/LoginPage';
import { DistributorSelectionPage } from '../pages/DistributorSelectionPage';
import { MainLayout } from '../components/MainLayout';
import { SalesDashboardPage } from '../features/dashboard/SalesDashboardPage';
import { ItemMasterPage } from '../features/master/ItemMasterPage';
import { DealerOrdersPage } from '../features/transactions/DealerOrdersPage';
import { ReportsPage } from '../features/reports/ReportsPage';
import { ClaimsPage } from '../features/claims/ClaimsPage';
import { SchemesPage } from '../features/schemes/SchemesPage';

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
      { path: 'transactions/sales-orders', element: <DealerOrdersPage /> },
      { path: 'schemes', element: <SchemesPage /> },
      { path: 'reports', element: <ReportsPage /> },
      { path: 'claims', element: <ClaimsPage /> }
    ]
  }
]);
