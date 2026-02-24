# Phase 2 - Architecture Blueprint (Implementation Baseline)

## Objective
Provide implementation guardrails for delivering the platform in Clean Architecture with operational controls.

## Implemented Architectural Components
1. WorkOrder lifecycle service path
2. Visit execution and evidence workflow
3. Approval and review flow
4. SLA evaluation engine and hosted processing
5. Escalation lifecycle
6. KPI/analytics/reporting read models
7. Portal read model with client scoping

## Data Model Coverage
Representative persisted components:
- WorkOrders
- Visits + evidence sub-entities
- Materials + transactions
- Escalations
- ChecklistTemplates
- AuditLogs and ApprovalRecords
- DailyPlans
- Assets
- SyncQueue/SyncConflict
- SystemSettings

## Critical Rules Enforced in Code
- UTC timestamps for business state transitions
- Policy/permission-based API authorization
- Domain state-transition guards
- Localized exception/message handling
- Additive migration policy

## Security Controls
- JWT authentication
- Permission-claim authorization policies
- Secrets expected outside source in production (`JWT_SECRET`)
- Request logging and centralized exception handling

## Non-Functional Direction
- Transaction behavior for command handlers
- Performance behavior instrumentation in MediatR pipeline
- Notification HTTP resilience (retry/circuit-breaker/timeout)
- CI gate with build + tests + documentation drift checks
