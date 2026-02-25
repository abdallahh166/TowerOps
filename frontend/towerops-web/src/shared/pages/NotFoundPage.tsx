import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <div className="screen-center">
      <div className="empty-state">
        <h1>404</h1>
        <p>The page does not exist in this workspace.</p>
        <Link to="/app/dashboard" className="primary-link-button">
          Back to dashboard
        </Link>
      </div>
    </div>
  );
}
