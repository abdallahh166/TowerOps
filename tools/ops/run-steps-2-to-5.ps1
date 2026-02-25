param(
    [string]$ReleaseTag = "v1.0.0-rc2",
    [switch]$CreateTag,
    [switch]$PushTag
)

$ErrorActionPreference = "Stop"

function Invoke-SmokeTest {
    param(
        [string]$Name,
        [string]$Filter
    )

    Write-Host "==> Running smoke: $Name" -ForegroundColor Cyan
    dotnet test TowerOps.sln `
        --configuration Debug `
        --no-build `
        --filter $Filter `
        --logger "console;verbosity=minimal"

    if ($LASTEXITCODE -ne 0) {
        throw "Smoke test failed: $Name"
    }
}

Write-Host "Step 2: Smoke Test Execution" -ForegroundColor Green
Invoke-SmokeTest -Name "Auth" -Filter "FullyQualifiedName~AuthControllerTests|FullyQualifiedName~LoginCommandHandlerTests|FullyQualifiedName~ForgotPasswordCommandHandlerTests|FullyQualifiedName~ResetPasswordCommandHandlerTests|FullyQualifiedName~ChangePasswordCommandHandlerTests|FullyQualifiedName~ChangePasswordCommandValidatorTests"
Invoke-SmokeTest -Name "Visits" -Filter "FullyQualifiedName~StartVisitCommandHandlerTests|FullyQualifiedName~SubmitVisitCommandHandlerTests|FullyQualifiedName~CancelVisitCommandHandlerTests|FullyQualifiedName~RescheduleVisitCommandHandlerTests|FullyQualifiedName~VisitQueryEfficiencyTests|FullyQualifiedName~GetVisitEvidenceStatusQueryHandlerTests|FullyQualifiedName~VisitTests|FullyQualifiedName~VisitLifecycleTests|FullyQualifiedName~VisitReviewFlowTests|FullyQualifiedName~VisitEditCancelRulesTests"
Invoke-SmokeTest -Name "WorkOrders" -Filter "FullyQualifiedName~CreateWorkOrderCommandHandlerTests|FullyQualifiedName~CustomerDecisionPortalGuardTests|FullyQualifiedName~WorkOrderTests"
Invoke-SmokeTest -Name "Import" -Filter "FullyQualifiedName~ImportSiteDataCommandHandlerTests|FullyQualifiedName~ImportCommandsRealFilesIntegrationTests|FullyQualifiedName~Sprint12DryRunReconciliationTests"
Invoke-SmokeTest -Name "Portal" -Filter "FullyQualifiedName~PortalQueriesTests|FullyQualifiedName~PortalReadRepositoryTests|FullyQualifiedName~CustomerDecisionPortalGuardTests"

Write-Host ""
Write-Host "Step 3: Go/No-Go + Follow-up Tickets" -ForegroundColor Green
Write-Host "- Update signoff checklist: docs/phase-2/17-go-no-go-checklist.md"
Write-Host "- Create follow-up issues from: docs/phase-2/18-follow-up-ticket-drafts.md"

Write-Host ""
Write-Host "Step 4/5: Release Tag" -ForegroundColor Green
if ($CreateTag) {
    $dirty = git status --porcelain
    if ($dirty) {
        throw "Working tree is not clean. Commit/stash before tagging."
    }

    $branch = (git rev-parse --abbrev-ref HEAD).Trim()
    if ($branch -ne "main") {
        throw "Tagging is restricted to main branch. Current branch: $branch"
    }

    git tag -a $ReleaseTag -m "TowerOps release candidate: smoke verified and go/no-go ready"
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create tag $ReleaseTag"
    }

    Write-Host "Created tag: $ReleaseTag" -ForegroundColor Yellow

    if ($PushTag) {
        git push origin $ReleaseTag
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to push tag $ReleaseTag"
        }
        Write-Host "Pushed tag: $ReleaseTag" -ForegroundColor Yellow
    }
}
else {
    Write-Host "Tagging skipped. Use -CreateTag (and optional -PushTag) when ready."
}

Write-Host ""
Write-Host "All requested steps completed successfully." -ForegroundColor Green
