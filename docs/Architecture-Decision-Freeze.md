# Architecture Direction Freeze (Day-1 Decisions)

Date: 2026-02-19

## Locked Decisions

1. **Auth moved to Application layer**
   - Login orchestration now runs through MediatR (`LoginCommand`, validator, handler).
   - API controller delegates only request/response translation.

2. **Single exception translation path**
   - API exception filter removed from controller pipeline.
   - Global middleware is the canonical exception-to-response translator.
   - Middleware now handles both domain and application validation exceptions.

3. **No duplicate service registrations across layers**
   - Pure domain/application services are owned by Application registration.
   - Infrastructure keeps I/O adapters and repository implementations.

4. **One mapping strategy at API edge**
   - API contract mapping standardized via dedicated mapper class (`AuthContractMapper`) for auth flow.
   - Direction: use this pattern consistently (or move to AutoMapper contract profiles) in subsequent refactors.

## Implementation Snapshot
- `AuthController` -> `LoginCommand` via mapper and `Mediator.Send`.
- `LoginCommandHandler` performs credential validation + token issuance using `IJwtTokenService` abstraction.
- `ApiExceptionFilter` no longer registered in `Program.cs`.
- Infrastructure duplicate registrations for domain services removed.

## Next Refactor Batch
- ✅ Apply contract mapper strategy across controllers (users/offices/visits plus remaining API endpoints).
- ✅ Extract shared editable-visit mutation workflow used by multiple visit command handlers.
- ▶ Next: enforce mapper/test template for new controllers by default and keep DoD closure evidence updated per sprint.
