# TowerOps Frontend

React + TypeScript frontend workspace for TowerOps operations users.

## Stack

- React 19
- Vite
- React Router
- TanStack Query
- Axios
- Zod

## Setup

```bash
npm install
npm run dev
```

Default local app URL:

- `http://localhost:5173`

## Environment

Copy `.env.example` to `.env` if needed.

- `VITE_API_BASE_URL=/api` uses Vite proxy and avoids CORS issues in local dev.
- `VITE_API_PROXY_TARGET=http://localhost:5277` should match backend launch URL.

## Build

```bash
npm run build
```

## Notes

- Login is wired to `POST /api/auth/login`.
- Protected routes use JWT from local storage.
- Dashboard uses `GET /api/kpi/operations`.
