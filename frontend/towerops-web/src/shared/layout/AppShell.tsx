import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../core/auth/useAuth";

const navigation = [
  { to: "/app/dashboard", label: "Dashboard" },
  { to: "/app/visits", label: "Visits" },
  { to: "/app/workorders", label: "Work Orders" },
  { to: "/app/sites", label: "Sites" },
  { to: "/app/reports", label: "Reports" },
];

export function AppShell() {
  const { user, logout } = useAuth();
  const today = new Intl.DateTimeFormat("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(new Date());

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand-block">
          <p className="brand-title">TowerOps</p>
          <p className="brand-subtitle">Seven Pictures</p>
        </div>

        <nav className="sidebar-nav">
          {navigation.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                isActive ? "nav-link nav-link-active" : "nav-link"
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        <button type="button" className="ghost-button" onClick={logout}>
          Sign out
        </button>
      </aside>

      <div className="workspace">
        <header className="top-bar">
          <div>
            <p className="top-bar-caption">Operations Console</p>
            <h2>
              Welcome, {user?.email} <span>{user?.role}</span>
            </h2>
          </div>

          <div className="top-bar-meta">
            <p>{today}</p>
            <p>Office: {user?.officeId}</p>
          </div>
        </header>

        <main className="page-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
