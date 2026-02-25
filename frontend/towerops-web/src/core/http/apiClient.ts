import axios from "axios";
import { getAccessToken } from "../auth/tokenStore";

const baseURL = import.meta.env.VITE_API_BASE_URL || "/api";

export const apiClient = axios.create({
  baseURL,
  timeout: 20_000,
});

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});
