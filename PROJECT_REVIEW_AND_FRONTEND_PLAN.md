# TelecomPM Frontend Review and Implementation Blueprint (API-informed)

This review is now based on the provided `TelecomPM API Documentation` (ASP.NET Core `net8.0`, JWT auth, policy/permission authorization, controller-based endpoints).

## 1) Product surface inferred from API
The backend already exposes a broad operations platform, so the frontend should be organized around **workflows**, not entities only:

- Operations execution: work orders, visits, daily plans, sync.
- Asset/site management: sites, assets, imports, maintenance views.
- Governance: escalations, approvals/rejections, checklist templates.
- Inventory: materials and stock operations.
- Oversight: KPI, analytics, reports.
- Administration: users, offices, roles, settings.
- External experience: client portal views/actions.

## 2) Frontend architecture recommendation

### Stack
- Next.js (App Router) + TypeScript
- Tailwind + component system (shadcn/ui)
- TanStack Query for server state + caching
- React Hook Form + Zod for DTO validation
- Axios client with auth interceptor and error normalizer
- Optional: Zustand for local UI state only (filters/view prefs)

### Why Next.js for TelecomPM (instead of plain React/Vite)
- **Routing + layouts for large domains:** TelecomPM has many bounded contexts (operations, admin, portal, analytics). App Router route groups/layouts keep these isolated and maintainable.
- **Built-in auth-friendly middleware:** Easy request-time redirects/guards for login and unauthorized pages in one framework.
- **Server Components for heavy data screens:** KPI/analytics/report pages can render faster with server data fetching where appropriate.
- **Hybrid rendering per page:** Use SSR for role-sensitive dashboards, static rendering for mostly static admin/help areas.
- **Production conventions included:** File-based routing, metadata, code splitting, image/font optimization, and deployment ergonomics reduce setup overhead.
- **Future-proof for portal SEO needs:** If client-portal pages later need indexing or faster first paint, Next.js already supports this without re-platforming.

### Why not Angular (for this project)
- **Not because Angular is bad**: Angular is strong for enterprise apps, but here it adds more framework overhead than needed for the first delivery phases.
- **Slower initial iteration for this team plan**: The current blueprint leans on React ecosystem utilities (TanStack Query, shadcn/ui patterns) that map directly to Next.js with less glue code.
- **Rendering flexibility gap for portal needs**: Next.js gives SSR/SSG/hybrid rendering primitives out-of-the-box for future client-portal SEO/performance without extra platform decisions.
- **Auth + route middleware ergonomics**: Next.js middleware and App Router layout boundaries make permission-aware route segmentation straightforward for TelecomPM's large policy surface.
- **Hiring/ecosystem velocity for this stack**: For mixed dashboard + portal products, React/Next has broader template/component/tooling reuse, reducing time-to-first-feature.
- **Migration risk control**: Starting with Next.js does not lock out Angular forever; API contracts remain framework-agnostic, so a future UI rewrite is possible if org standards require Angular.

When Angular *would* be preferable:
- Your organization has strict Angular standardization.
- Existing internal component libraries are Angular-first.
- Team experience is predominantly Angular and delivery speed would be higher with it.

### App structure
```text
src/
  app/
    (auth)/login
    operations/
      work-orders/
      visits/
      daily-plans/
      sync/
    inventory/materials/
    governance/
      escalations/
      checklist-templates/
    assets/
      sites/
      assets/
    analytics/
      kpi/
      analytics/
      reports/
    admin/
      users/
      offices/
      roles/
      settings/
    portal/
  features/
    auth/
    permissions/
    work-orders/
    visits/
    sites/
    materials/
    analytics/
    users/
  lib/
    api/client.ts
    api/errors.ts
    auth/token.ts
    auth/claims.ts
    query/client.ts
  types/
    api.ts
    auth.ts
    permissions.ts
```

## 3) Auth and permission model (critical)
Backend authorization is policy/permission-claim driven (not role hardcoding). Frontend should mirror this exactly.

### Required frontend auth behavior
1. Login via `POST /api/auth/login`.
2. Store JWT securely (prefer httpOnly cookie if gateway supports; otherwise in-memory + refresh pattern).
3. Decode permission claims to gate routes, menus, and actions.
4. Distinguish:
   - unauthenticated (redirect to login),
   - authenticated but unauthorized (403 view),
   - authenticated and authorized.

### Permission map seed
Define a permission constant map in frontend for:
- WorkOrders: `CanManageWorkOrders`, `CanViewWorkOrders`
- Visits: `CanManageVisits`, `CanViewVisits`, `CanReviewVisits`
- Escalations: `CanManageEscalations`, `CanViewEscalations`
- KPI/Analytics/Reports: `CanViewKpis`, `CanViewAnalytics`, `CanViewReports`
- Admin: `CanManageUsers`, `CanViewUsers`, `CanManageOffices`, `CanManageSettings`
- Site/Asset/Portal/Materials: `CanManageSites`, `CanViewSites`, `CanViewPortal`, `CanViewMaterials`, `CanManageMaterials`

## 4) API-to-screen mapping (MVP-first)

### A. Auth module
- Login, forgot password, reset password, change password.
- Endpoints:
  - `POST /api/auth/login`
  - `POST /api/auth/forgot-password`
  - `POST /api/auth/reset-password`
  - `POST /api/auth/change-password`

### B. Work Orders module
- List/detail, create, assign, lifecycle actions (start/complete/close/cancel/submission/acceptance), signature capture/view.
- Endpoints under `/api/workorders`.
- UI notes:
  - timeline state machine widget for lifecycle transitions.
  - per-action permission checks.

### C. Visits module
- Visit detail, scheduling, execution flow (start/checkin/checkout/complete/submit), review queue, approvals/rejections/correction.
- Evidence capture: photos, signature, checklist items, issues, readings.
- Endpoints under `/api/visits`.
- UI notes:
  - engineer mode (field workflow) + reviewer mode (approval queue).
  - explicit status chips for pending-review/scheduled/completed.

### D. Sites + Assets module
- Site detail/location/maintenance list, assignment and ownership operations.
- Asset by site, asset history, service/fault/replace operations.
- Import operations can be admin tools with guarded forms and job-result feedback.

### E. Inventory (Materials)
- Materials listing and detail, low-stock dashboard, stock add/reserve/consume operations.
- Strong validation on stock operations + optimistic update rollback.

### F. Oversight (KPI/Analytics/Reports)
- KPI cards from `/api/kpi/operations`.
- Analytics dashboards from `/api/analytics/*`.
- Reporting views from `/api/reports/*` with export option.

### G. Administration
- Users CRUD and role/activation flows.
- Offices CRUD + statistics.
- Roles + permissions management.
- Settings retrieval/update + test service action.

### H. Client Portal
- Separate route group/layout for portal users.
- Dashboard, sites, workorders, SLA report, evidence, and accept/reject actions.

## 5) Contract and UX standards to enforce

### API client standards
- Unified response parser for success/failure envelope.
- Global error normalization for validation dictionary + localized message strings.
- Query key conventions by bounded context (`['workorders', id]`, etc.).

### State/loading standards
- Every data page must include loading, empty, error, and retry states.
- Mutation buttons must have pending/disabled state and idempotency protection.

### Localization and dates
- Support `en-US` and `ar-EG` in UI and locale switching.
- Render UTC timestamps in user locale while preserving server UTC values.
- Respect API language mechanisms (`Accept-Language`, query, cookie strategy).

## 6) Suggested phased delivery

### Phase 1 (2 weeks) — Foundation
- Next.js app shell + auth + permission guard layer.
- API client, error model, query client, i18n scaffolding.
- Implement: login, dashboard shell, unauthorized page.

### Phase 2 (3 weeks) — Core operations
- Work orders + visits execution and review flows.
- Sites overview and basic assets lookup.
- KPI operations dashboard.

### Phase 3 (2 weeks) — Admin and inventory
- Users/offices/roles/settings.
- Materials and stock actions.
- Reports + analytics pages.

### Phase 4 (1–2 weeks) — Hardening
- Accessibility pass, role matrix QA, e2e smoke tests.
- Performance tuning (table virtualization, request dedupe, caching strategy).

## 7) Immediate implementation checklist
1. Create typed API contracts for auth/workorders/visits first.
2. Implement permission-aware route guard + action guard components.
3. Build reusable data-table with server pagination/filter/sort adapters.
4. Build mutation action bar pattern (confirm modal + reason input where needed).
5. Add audit-friendly action logs in UI for critical transitions (approve/reject/close/cancel).

## 8) Key risks and mitigations
- **Risk:** Very large endpoint surface can create inconsistent UX.
  - **Mitigation:** enforce shared page templates and action patterns by module.
- **Risk:** Permission complexity leads to hidden/visible mismatch.
  - **Mitigation:** central permission hook + policy test matrix.
- **Risk:** Field workflows (visits) need offline resilience.
  - **Mitigation:** plan progressive enhancement using `/api/sync` endpoints in v2.

---

If you want, next step I can produce a concrete **frontend starter repository structure with initial pages and typed API client stubs** aligned to these exact endpoint groups.
