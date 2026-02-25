import { createContext } from "react";
import type { LoginRequest, LoginResponse } from "./authApi";

export type AuthState = {
  accessToken: string;
  expiresAtUtc: string;
  userId: string;
  email: string;
  role: string;
  officeId: string;
  requiresPasswordChange: boolean;
};

export type AuthContextValue = {
  user: AuthState | null;
  isAuthenticated: boolean;
  isBootstrapping: boolean;
  login: (payload: LoginRequest) => Promise<LoginResponse>;
  logout: () => void;
};

export const AuthContext = createContext<AuthContextValue | undefined>(undefined);
