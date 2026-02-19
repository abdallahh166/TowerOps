# Architecture Consistency & Duplication Review

Date: 2026-02-19

## Scope
- API/Application/Domain/Infrastructure architecture boundaries
- Duplication across handlers, controllers, validators, and services
- SOLID consistency and responsibility overlap

## 1) Code Duplication Findings

### 1.1 Repeated controller command/query mapping boilerplate
**Symptoms**
- Controllers repeatedly map request contracts to commands with nearly identical imperative blocks.
- Same flow repeated: create command/query -> `Mediator.Send` -> `HandleResult`/`CreatedAtAction`.

**Examples**
- `UsersController` (`Create`, `Update`, `Delete`, role/activate/deactivate, queries)
- `OfficesController` (same pattern across CRUD + stats)
- `VisitsController` repeated command creation for lifecycle and evidence endpoints.

**Impact**
- High copy-paste risk.
- Increased maintenance cost for shape changes in command objects.

**Refactoring**
- Introduce AutoMapper mapping between API contracts and commands.
- Keep controllers thin by forwarding mapped commands/queries.
- Optionally add endpoint helper methods for common Created/Ok flows.

---

### 1.2 Repeated visit mutation workflow in multiple command handlers
**Symptoms**
The following handlers repeat the same lifecycle skeleton:
1. Load visit by ID
2. return "Visit not found" on null
3. guard `visit.CanBeEdited()`
4. mutate aggregate
5. update repository + save unit-of-work
6. map entity to DTO
7. catch exception -> `Result.Failure`

**Examples**
- `AddIssueCommandHandler`
- `AddChecklistItemCommandHandler`
- `UpdateChecklistItemCommandHandler`
- `AddPhotoCommandHandler`
- `AddReadingCommandHandler` (same pattern without try/catch wrapper)

**Impact**
- Behavior drift risk (different error text, exception wrapping, and transactional behavior).
- Hard to enforce consistent error handling and telemetry.

**Refactoring**
- Extract a shared application service / workflow helper for "editable visit mutation".
- Standardize failure contracts and exception translation once.

---

### 1.3 Repeated try/catch + string error translation
**Symptoms**
- Multiple handlers/services catch broad `Exception` and convert to string-based `Result.Failure(...)`.
- Same pattern also appears in infrastructure services with logger + rethrow.

**Examples**
- Visit command handlers (AddIssue/AddChecklist/UpdateChecklist/AddPhoto).
- User/office command handlers.
- Infrastructure service methods (Email/Excel/Report generation).

**Impact**
- Inconsistent error semantics.
- Loss of typed error context and potential information leakage (`ex.Message` surfaced directly).

**Refactoring**
- Prefer centralized exception handling in pipeline/middleware.
- Use typed domain/application errors instead of raw exception messages.

---

### 1.4 Mapping duplication (manual + AutoMapper)
**Symptoms**
- AutoMapper profile already defines mappings for visits/materials/users/sites/workorders.
- Controllers still manually map request contracts to commands.
- Some handlers manually map/calculated DTO structures in-line.

**Impact**
- Dual mapping strategy increases inconsistency probability.

**Refactoring**
- Adopt one mapping policy:
  - API contracts -> commands/queries via AutoMapper.
  - domain -> DTO via AutoMapper in handlers.
- Reserve manual mapping only for true computed projections.

## 2) Responsibility Overlap Findings

### 2.1 Validation overlap across layers
**Symptoms**
- API layer has `ValidateModelStateFilter`.
- Application layer has FluentValidation `ValidationBehavior` pipeline.
- Some controller methods add manual input guards (e.g., auth endpoint checks).

**Impact**
- Same concern handled in multiple places, producing different error shapes.

**Refactoring**
- Keep syntactic payload validation at API edge.
- Keep semantic/business validation in Application validators.
- Ensure one consistent error response contract.

---

### 2.2 Duplicate service registrations across Application and Infrastructure
**Symptoms**
- Same domain service interfaces are registered in **both** layers:
  - `IVisitValidationService`
  - `ISiteAssignmentService`
  - `IVisitDurationCalculatorService`
  - `IPhotoChecklistGeneratorService`

**Impact**
- Ambiguous ownership and last-registration-wins behavior.
- Architectural boundary confusion.

**Refactoring**
- Register pure domain/application services in one place only (prefer Application).
- Keep Infrastructure registrations for I/O implementations only.

## 3) Abstraction Issues

### 3.1 Authentication implementation bypasses Application layer
**Symptoms**
- `AuthController` directly queries repository and issues tokens.

**Impact**
- Business/auth rules bypass CQRS pipeline (logging/validation/performance/transaction behaviors).
- Harder to evolve toward MFA/password auth/revocation.

**Refactoring**
- Move login into Application command/query (`LoginCommandHandler`) and keep controller orchestration-only.

---

### 3.2 Weak credential model for login
**Symptoms**
- Login uses `email + phoneNumber` plain comparison.

**Impact**
- Not production-grade authentication security model.

**Refactoring**
- Introduce credential aggregate/policy (hashed password + rotation/lockout).
- Keep token generation service, but drive via application auth workflow.

---

### 3.3 Potential over-engineering around mixed exception handling layers
**Symptoms**
- API filter + middleware + MediatR behaviors + handler-level try/catch all overlap.

**Impact**
- Hard to reason where exceptions are transformed; inconsistent responses.

**Refactoring**
- Define one canonical exception translation boundary (middleware + problem details).
- Remove redundant per-handler broad catches unless adding business context.

## 4) SOLID Assessment (High-level)

- **S (Single Responsibility):** Violated in controllers/handlers with repeated mapping + orchestration + validation checks.
- **O (Open/Closed):** Mostly acceptable, but duplicated flows require touching many files for one policy change.
- **L (Liskov):** No major inheritance abuse detected.
- **I (Interface Segregation):** Mixed; several service interfaces are fine-grained, but auth still too ad-hoc.
- **D (Dependency Inversion):** Generally good (interfaces), but layering bypass in auth controller reduces benefits.

## 5) Recommended Refactoring Roadmap

### Phase A (quick wins)
1. Remove duplicate service registrations from Infrastructure for pure domain/application services.
2. Normalize visit command handlers with a shared editable-visit workflow helper.
3. Standardize exception-to-response translation path.

### Phase B (architectural hardening)
1. Introduce `LoginCommand` + `LoginCommandHandler` in Application.
2. Move auth validation rules to FluentValidation validators.
3. Add dedicated `IAuthService` abstraction for token issuance + claim policy.

### Phase C (consistency/scale)
1. Adopt contract->command AutoMapper mappings in API to remove repetitive manual mapping.
2. Introduce integration tests for error shape consistency across filters/middleware/pipeline.
3. Add architecture tests (layer dependency rules, duplicate registration checks).

## 6) Example Improved Code (pattern)

```csharp
// Application layer
public sealed record MutateEditableVisitCommand<TOut>(Guid VisitId, Func<Visit, TOut> Mutate)
    : IRequest<Result<TOut>>;

public sealed class MutateEditableVisitHandler<TOut> : IRequestHandler<MutateEditableVisitCommand<TOut>, Result<TOut>>
{
    private readonly IVisitRepository _repo;
    private readonly IUnitOfWork _uow;

    public async Task<Result<TOut>> Handle(MutateEditableVisitCommand<TOut> request, CancellationToken ct)
    {
        var visit = await _repo.GetByIdAsync(request.VisitId, ct);
        if (visit is null) return Result.Failure<TOut>("Visit not found");
        if (!visit.CanBeEdited()) return Result.Failure<TOut>("Visit cannot be edited");

        var result = request.Mutate(visit);
        await _repo.UpdateAsync(visit, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(result);
    }
}
```

This removes repeated editable-visit boilerplate from many handlers.
