# Phase 0 — Security Baseline v1

## Mandatory Baseline
1. Role-based access control (RBAC) across all workflow actions.
2. Segregation of duties between execution, review, and approval.
3. No secret keys/passwords in source-controlled configuration.
4. Environment-based CORS restrictions (dev/UAT/prod).
5. Structured error model (no string-parsing for access/business errors).
6. Full audit trail for Work Order/Visit/Approval transitions.

## Baseline Controls
- Authentication: JWT bearer with environment-managed secrets.
- Authorization: policy-based checks by role + workflow stage.
- Audit: who/when/what for create/update/approve/reject/escalate/reopen.
- Data protection: validate file uploads and metadata for evidence.
- Operational security: production hardening checklist is a go-live gate.

## Exit Criteria for Phase 0
- RBAC matrix approved and signed.
- Security owner assigned for policy governance.
- Secrets management mechanism approved.
- CORS profile approved for each environment.
