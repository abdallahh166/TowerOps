# TowerOps Operational Workflow

This document describes how real users operate TowerOps day to day, with explicit workflow/state diagrams aligned to current implementation.

## 1) Daily User Workflow (Role Interaction)

```mermaid
flowchart TD
    A[Admin] --> A1[Configure settings, roles, users, offices]
    A1 --> B[Office Manager / Supervisor]

    B --> B1[Import or maintain site master data]
    B1 --> B2[Create Daily Plan]
    B2 --> B3[Assign sites to engineers manually]
    B3 --> B4[Get suggested route order]
    B4 --> B5[Adjust order if needed]
    B5 --> B6[Publish plan]

    B6 --> C[Field Engineer]
    C --> C1[Open assigned visits]
    C1 --> C2[GPS check-in]
    C2 --> C3[Start visit]
    C3 --> C4[Checklist auto-generated from active template]
    C4 --> C5[Collect evidence: photos, readings, checklist]
    C5 --> C6[Log issues and materials]
    C6 --> C7[Capture signatures]
    C7 --> C8[Complete + submit visit]

    C8 --> D[Supervisor / Manager Review]
    D -->|Approve| E[Approved Visit]
    D -->|Request correction| C
    D -->|Reject| F[Rejected Visit]

    E --> G[Work Order lifecycle if required]
    G --> H[KPI + Reports + Exports]

    H --> I[Client Portal User]
    I --> I1[Read-only dashboard/sites/workorders/SLA]
```

## 2) Visit Lifecycle State Machine

```mermaid
stateDiagram-v2
    [*] --> Scheduled

    Scheduled --> InProgress: StartVisit
    Scheduled --> Cancelled: Cancel
    Scheduled --> Scheduled: Reschedule

    InProgress --> Completed: CompleteVisit
    InProgress --> Cancelled: Cancel

    Completed --> Submitted: Submit
    Completed --> Cancelled: Cancel

    Submitted --> UnderReview: StartReview
    UnderReview --> Approved: Approve
    UnderReview --> Rejected: Reject
    UnderReview --> NeedsCorrection: RequestCorrection

    NeedsCorrection --> Submitted: Submit

    Approved --> [*]
    Rejected --> [*]
    Cancelled --> [*]
```

Notes:
- Check-in/out is tracked with GPS and distance-from-site.
- Suspicious check-in (outside radius) is flagged, not blocked by default.
- Evidence completeness is enforced before submit.

## 3) Work Order Lifecycle State Machine

```mermaid
stateDiagram-v2
    [*] --> Created

    Created --> Assigned: Assign
    Created --> Cancelled: Cancel

    Assigned --> InProgress: Start
    Assigned --> Cancelled: Cancel

    InProgress --> PendingInternalReview: Complete
    InProgress --> Cancelled: Cancel

    PendingInternalReview --> PendingCustomerAcceptance: SubmitForCustomerAcceptance
    PendingInternalReview --> Closed: Close

    PendingCustomerAcceptance --> Closed: AcceptByCustomer
    PendingCustomerAcceptance --> Rework: RejectByCustomer
    PendingCustomerAcceptance --> Closed: Close

    Rework --> Assigned: Assign
    Rework --> Cancelled: Cancel

    Closed --> [*]
    Cancelled --> [*]
```

Notes:
- For equipment-only sites, creating a `TowerInfrastructure` scope work order is blocked.
- Customer acceptance is explicit and auditable.

## 4) Escalation Lifecycle State Machine

```mermaid
stateDiagram-v2
    [*] --> Submitted
    Submitted --> UnderReview: Review
    UnderReview --> Approved: Approve
    UnderReview --> Rejected: Reject
    Approved --> Closed: Close
    Rejected --> Closed: Close
    Closed --> [*]
```

## 5) Offline Sync Workflow (Field Reality)

```mermaid
flowchart LR
    A[Mobile Action Offline] --> B[Queue item on device]
    B --> C[POST /api/sync batch]
    C --> D[Server stores SyncQueue entries]
    D --> E[Processor orders by CreatedOnDeviceUtc]
    E --> F{Conflict?}
    F -->|No| G[Mark Processed]
    F -->|Yes| H[Mark Conflict + create SyncConflict]
    E --> I{Unhandled error?}
    I -->|Yes| J[Mark Failed + increment retry]
```

Conflict examples:
- Visit already submitted
- Duplicate photo
- Duplicate reading (server wins)

## 6) Single-Page Swimlane (Training View)

```mermaid
flowchart LR
    subgraph Admin["Admin Lane"]
        A1[Configure system settings]
        A2[Manage roles and permissions]
        A3[Create/invite users]
    end

    subgraph Manager["Office Manager / Supervisor Lane"]
        M1[Import and maintain sites]
        M2[Create daily plan]
        M3[Assign sites to engineers]
        M4[Review suggested route order]
        M5[Publish plan]
        M6[Review submitted visits]
        M7[Create/assign work orders]
        M8[Handle escalations]
        M9[Track KPI and exports]
    end

    subgraph Engineer["Field Engineer Lane"]
        E1[Open assigned visits]
        E2[GPS check-in]
        E3[Start visit]
        E4[Fill checklist and evidence]
        E5[Add readings, photos, issues, materials]
        E6[Capture signatures]
        E7[Complete and submit visit]
        E8[Execute work order]
        E9[Offline sync when connected]
    end

    subgraph Client["Client Portal Lane"]
        C1[View portal dashboard]
        C2[View site status and visits]
        C3[View work order and SLA reports]
    end

    A1 --> A2 --> A3 --> M1
    M1 --> M2 --> M3 --> M4 --> M5 --> E1
    E1 --> E2 --> E3 --> E4 --> E5 --> E6 --> E7 --> M6
    M6 -->|Approved| M7
    M6 -->|Needs correction| E4
    M7 --> E8 --> M8 --> M9
    E9 --> M6
    M9 --> C1 --> C2 --> C3
```
