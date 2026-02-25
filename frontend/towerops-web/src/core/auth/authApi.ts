import { apiClient } from "../http/apiClient";

export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
  expiresAtUtc: string;
  userId: string;
  email: string;
  role: string;
  officeId: string;
  requiresPasswordChange: boolean;
};

export async function login(payload: LoginRequest) {
  const response = await apiClient.post<LoginResponse>("/auth/login", payload);
  return response.data;
}
