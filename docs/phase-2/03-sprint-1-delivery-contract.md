# Sprint 1 — Delivery Contract

## Scope
1. `POST /api/workorders`
2. `POST /api/workorders/{woId}/assign`
3. Role policy checks for dispatcher actions
4. Response SLA clock initialization in UTC

## Acceptance Criteria
- WO cannot be created without SLA class and site code.
- Assignment is allowed only from `Created` state.
- Unauthorized role receives policy-based forbidden response.
- `CreatedAtUtc` and `AssignedAtUtc` persisted in UTC.
- Audit record generated for create and assign actions.

## Test Matrix
1. Valid WO creation (happy path)
2. Missing mandatory field (validation fail)
3. Invalid state transition to assign (business rule fail)
4. Unauthorized assign attempt (security fail)
5. UTC persistence assertion (integration test)

## Demo Checklist
- Create WO from API
- Assign WO
- Show stored UTC timestamps
- Show audit log entries
- Show policy rejection for unauthorized role


## Implementation References
- Controller: `src/TelecomPm.Api/Controllers/WorkOrdersController.cs`
- Commands: `CreateWorkOrderCommand`, `AssignWorkOrderCommand`
- Domain: `src/TelecomPM.Domain/Entities/WorkOrders/WorkOrder.cs`
- Policy wiring: `src/TelecomPm.Api/Authorization/ApiAuthorizationPolicies.cs`
