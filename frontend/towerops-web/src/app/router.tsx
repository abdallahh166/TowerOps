import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import type { ReactElement } from "react";
import { LoginPage } from "../features/auth/LoginPage";
import { DashboardPage } from "../features/dashboard/DashboardPage";
import { ReportsPage } from "../features/reports/ReportsPage";
import { SitesPage } from "../features/sites/SitesPage";
import { VisitsPage } from "../features/visits/VisitsPage";
import { WorkOrdersPage } from "../features/workorders/WorkOrdersPage";
import { AppShell } from "../shared/layout/AppShell";
import { NotFoundPage } from "../shared/pages/NotFoundPage";
import { useAuth } from "../core/auth/useAuth";

function ProtectedRoute({ children }: { children: ReactElement }) {
  const { isAuthenticated, isBootstrapping } = useAuth();

  if (isBootstrapping) {
    return <div className="screen-loader">Loading workspace...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function PublicOnlyRoute({ children }: { children: ReactElement }) {
  const { isAuthenticated } = useAuth();
  if (isAuthenticated) {
    return <Navigate to="/app/dashboard" replace />;
  }

  return children;
}

export function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/login"
          element={
            <PublicOnlyRoute>
              <LoginPage />
            </PublicOnlyRoute>
          }
        />

        <Route
          path="/app"
          element={
            <ProtectedRoute>
              <AppShell />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="/app/dashboard" replace />} />
          <Route path="dashboard" element={<DashboardPage />} />
          <Route path="visits" element={<VisitsPage />} />
          <Route path="workorders" element={<WorkOrdersPage />} />
          <Route path="sites" element={<SitesPage />} />
          <Route path="reports" element={<ReportsPage />} />
        </Route>

        <Route path="/" element={<Navigate to="/app/dashboard" replace />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
}
