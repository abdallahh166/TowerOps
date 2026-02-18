# Phase 1 — Domain Model v1

## Core Aggregates

## 1) WorkOrder
- `WoNumber`
- `SiteCode`
- `OfficeCode`
- `Severity` (P1..P4)
- `Status` (Created, Assigned, InProgress, PendingReview, PendingCustomerAcceptance, Closed, Rejected, Rework, Cancelled)
- `CreatedAtUtc`, `ClosedAtUtc`
- `ResponseDeadlineUtc`, `ResolutionDeadlineUtc`
- `CurrentApprovalStage`

## 2) Visit
- `VisitId`
- `WorkOrderId`
- `EngineerId`
- `CapturedAtUtc`
- `ExecutionStatus`
- `ChecklistItems[]`
- `Readings[]`
- `Photos[]`

## 3) ApprovalRecord
- `WorkOrderId`
- `WorkflowType` (CM/BM)
- `StageName`
- `ReviewerRole`
- `Decision` (Approve/Reject/Rework)
- `DecisionAtUtc`
- `Reason`

## 4) EscalationRecord
- `WorkOrderId`
- `Severity`
- `EscalationLevel`
- `TriggeredByRule`
- `FinancialImpact`
- `SlaImpactPercent`
- `PreviousActions`
- `RecommendedDecision`
- `SubmittedAtUtc`

## 5) EvidencePackage
- `WorkOrderId`
- `VisitId`
- `BeforePhotosCount`
- `AfterPhotosCount`
- `ChecklistComplete`
- `ReadingsComplete`
- `CompletenessPercent`

## Domain Services
- `SlaClockService` (response/resolution/breach by UTC)
- `ApprovalPolicyService` (CM/BM staged policy)
- `EscalationPolicyService` (rule-based routing)
- `EvidenceValidationService` (hard validation rules)

## Key Invariants
1. Every WorkOrder has exactly one SLA class.
2. No escalation accepted with missing mandatory fields.
3. Rejection always routes to `Rework` with preserved history.
4. Final closure requires approval chain completion + required evidence.
