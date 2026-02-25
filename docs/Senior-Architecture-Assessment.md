# Senior .NET Architecture Assessment â€“ TowerOps

This is a point-in-time assessment snapshot. Some findings may already be resolved. For current runtime behavior, use `docs/Api-Doc.md`, `docs/Application-Doc.md`, and `docs/Domain-Doc.md`.

## Scope
- Reviewed API composition, Application layer (CQRS + behaviors), Infrastructure/EF Core persistence, and operational settings.
- Review based on current code snapshot.

## 1) Architecture â€” **7/10**

### What is strong
- Clear layered structure (`Api`, `Application`, `Domain`, `Infrastructure`) with DI entry points per layer.
- CQRS/MediatR pipeline with validation, performance logging, exception handling, and transaction behavior.
- Domain model appears rich and test-backed (entities, value objects, domain services, specifications).

### Key concerns
- **Service registration duplication/ambiguity**: some interfaces are registered in both `Application` and `Infrastructure`; final winner depends on registration order.
- **Transaction strategy overlap**: transaction pipeline + per-handler `SaveChangesAsync` creates unclear commit responsibility.
- **String-based command detection** in transaction behavior (`EndsWith("Command")`) is brittle and refactor-hostile.

### Suggested refactoring
- Introduce explicit marker interfaces (`ICommand<T>`, `IQuery<T>`) and bind transaction behavior to `ICommand<T>` only.
- Centralize persistence commit model:
  - Option A: handlers mutate only, pipeline commits once.
  - Option B: handlers own commit; remove transaction pipeline.
- Keep service ownership explicit:
  - Application layer registers only pure/in-process services.
  - Infrastructure layer registers external/I/O-backed implementations.

## 2) Code Quality â€” **7/10**

### What is strong
- Good separation in handlers/validators and use of async/cancellation.
- Test projects exist for domain/application/infrastructure.
- Fluent validations and DTO boundary mapping are in place.

### Anti-patterns / smells
- **Stringly-typed error mapping** in `ApiControllerBase.HandleFailure` infers HTTP status by searching substrings in error text.
- Potential drift from duplicate exception handling paths (`ApiExceptionFilter` + `ExceptionHandlingMiddleware` + base controller error mapping semantics).

### Suggested refactoring
- Replace string error parsing with structured errors (`ErrorCode`, `ErrorType`, ProblemDetails factory).
- Consolidate error handling strategy:
  - Prefer a single global exception -> `ProblemDetails` pipeline.
  - Keep command/query failures as typed `Result<T>` with error codes.

## 3) Security â€” **4/10**

### High-risk issues
- **Hardcoded sensitive settings** in `appsettings.json` (JWT secret, SMTP password placeholder style, storage connection string format).
- **Very permissive CORS policy** (`AllowAnyOrigin/AnyHeader/AnyMethod`) in runtime pipeline.
- **Missing endpoint authorization attributes/policies** on controllers despite JWT authentication being configured.

### Suggested refactoring
- Move secrets to environment variables / secret manager / vault. Keep only non-sensitive defaults in source.
- Replace `AllowAll` with named, environment-based origin allowlists.
- Apply policy-based authorization at controller/action level (roles/claims per use case).

## 4) Performance â€” **6/10**

### What is strong
- Query-side repository methods support `AsNoTracking`.
- Common indexes are configured (e.g., `Visits` by status/date/engineer/site).
- Performance behavior warns on slow requests.

### Concerns
- Transaction flow may introduce redundant `SaveChanges` cycle.
- Domain events are dispatched inside `SaveChangesAsync` before database commit finalization, which can increase latency and consistency risk for side effects.
- Potential for large in-memory materialization in generic methods returning `ToListAsync` without projection in some paths.

### Suggested refactoring
- Use outbox pattern for integration side effects and dispatch after commit.
- Add projection-first query handlers (AutoMapper `ProjectTo`) for read-heavy endpoints.
- Add targeted load/perf tests for top APIs (visits list, analytics queries).

## 5) EF Core Usage â€” **7/10**

### What is strong
- Configurations are explicit and mostly robust (indexes, conversions, owned types).
- Soft-delete global query filter exists.
- Audit interceptor captures created/updated/deleted state.

### Risks / anti-patterns
- Domain event dispatching in `DbContext.SaveChangesAsync` before commit can violate reliability expectations.
- Soft delete strategy is split between repository delete methods and interceptor handling deleted state; this can become inconsistent across code paths.

### Suggested refactoring
- Standardize soft-delete behavior in one place (prefer interceptor or explicit domain command path, not both).
- Consider EF execution strategy + resilient transactions for cloud SQL transient faults.
- Ensure migration pipeline and startup validation for schema drift in production.

## 6) Production Readiness â€” **6/10**

### Good baseline
- Serilog configured; health check endpoint exists.
- Swagger configured and available for development.
- Layered tests exist.

### Gaps to close
- No clear hardening profile separation for dev/staging/prod (CORS, secrets, logging sensitivity).
- Need stronger authorization model and security defaults before public deployment.
- Missing explicit operational patterns: distributed tracing/metrics dashboards, retry/circuit policies for external services, runbook-level failure handling.

### Suggested refactoring
- Add environment-specific config validation at startup (fail-fast for missing secrets and required endpoints).
- Introduce OpenTelemetry tracing + metrics and log correlation IDs.
- Add readiness/liveness checks beyond DB (storage/email dependencies where relevant).

---

## Prioritized Improvement Plan

### P0 (Immediate â€“ before production exposure)
1. Remove secrets from source and rotate JWT/SMTP/storage credentials.
2. Lock down CORS by environment and origin.
3. Enforce `[Authorize]` + policy-based authorization on all sensitive endpoints.
4. Replace string-based error handling with structured ProblemDetails + error codes.

### P1 (Next sprint)
1. Refactor command/query contracts to marker interfaces and remove name-based transaction checks.
2. Clarify commit ownership (single save/commit responsibility).
3. Unify service registration boundaries to remove duplicate interface mappings.
4. Add endpoint-level security/integration tests for authz rules.

### P2 (Stabilization)
1. Move domain/integration side effects to outbox pattern and post-commit processing.
2. Add query projections and benchmark hot paths.
3. Extend observability: tracing, metrics, SLOs, alerting.

### P3 (Scale & governance)
1. Add architecture decision records (ADRs) for transaction strategy, error model, and eventing.
2. Introduce static analyzers/security scanning in CI (SAST/dependency scanning).
3. Add performance regression gates in CI for critical endpoints.
