import { useMemo, useState, type PropsWithChildren } from "react";
import { login as loginApi } from "./authApi";
import { clearAccessToken, setAccessToken } from "./tokenStore";
import { AuthContext, type AuthState, type AuthContextValue } from "./authContext";

const SESSION_KEY = "towerops.session";

function hydrateSession() {
  const raw = localStorage.getItem(SESSION_KEY);
  if (!raw) {
    return null;
  }

  try {
    const parsed = JSON.parse(raw) as AuthState;
    if (!parsed.accessToken || !parsed.expiresAtUtc) {
      return null;
    }

    if (new Date(parsed.expiresAtUtc).getTime() <= Date.now()) {
      return null;
    }

    setAccessToken(parsed.accessToken);
    return parsed;
  } catch {
    return null;
  }
}

function persistSession(session: AuthState | null) {
  if (!session) {
    localStorage.removeItem(SESSION_KEY);
    clearAccessToken();
    return;
  }

  localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  setAccessToken(session.accessToken);
}

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<AuthState | null>(() => hydrateSession());

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      isBootstrapping: false,
      login: async (payload) => {
        const response = await loginApi(payload);
        const session: AuthState = {
          accessToken: response.accessToken,
          expiresAtUtc: response.expiresAtUtc,
          userId: response.userId,
          email: response.email,
          role: response.role,
          officeId: response.officeId,
          requiresPasswordChange: response.requiresPasswordChange,
        };
        setUser(session);
        persistSession(session);
        return response;
      },
      logout: () => {
        setUser(null);
        persistSession(null);
      },
    }),
    [user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
