import { z } from "zod";
import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../core/auth/useAuth";
import { getApiErrorMessage } from "../../core/http/errorMessage";

const loginSchema = z.object({
  email: z.email("Enter a valid email address."),
  password: z.string().min(6, "Password must be at least 6 characters."),
});

export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [warning, setWarning] = useState<string | null>(null);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setWarning(null);

    const validation = loginSchema.safeParse({ email, password });
    if (!validation.success) {
      setError(validation.error.issues[0]?.message ?? "Invalid login payload.");
      return;
    }

    try {
      setBusy(true);
      const response = await login(validation.data);
      if (response.requiresPasswordChange) {
        setWarning(
          "Password change is required by policy. Continue to dashboard and update credentials.",
        );
      }
      navigate("/app/dashboard", { replace: true });
    } catch (submissionError) {
      setError(getApiErrorMessage(submissionError, "Login failed."));
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="login-screen">
      <section className="login-hero">
        <p className="hero-eyebrow">TowerOps Control Center</p>
        <h1>Field Operations, One Command Surface</h1>
        <p>
          Centralize PM/CM execution, evidence compliance, site workflows, and SLA
          outcomes for subcontractor teams.
        </p>
      </section>

      <section className="login-card">
        <h2>Sign in</h2>
        <p>Use your TowerOps account to access the operations workspace.</p>

        <form onSubmit={onSubmit} className="form-stack">
          <label>
            <span>Email</span>
            <input
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="engineer@towerops.com"
              autoComplete="username"
              required
            />
          </label>

          <label>
            <span>Password</span>
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Enter your password"
              autoComplete="current-password"
              required
            />
          </label>

          {error ? <p className="message message-error">{error}</p> : null}
          {warning ? <p className="message message-warning">{warning}</p> : null}

          <button type="submit" disabled={busy}>
            {busy ? "Signing in..." : "Sign in"}
          </button>
        </form>
      </section>
    </div>
  );
}
