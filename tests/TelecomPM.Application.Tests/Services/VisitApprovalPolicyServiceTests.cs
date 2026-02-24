using FluentAssertions;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Enums;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class VisitApprovalPolicyServiceTests
{
    private readonly VisitApprovalPolicyService _service = new();

    [Fact]
    public void CanReviewVisit_ShouldDenyTechnicianForBmVisit()
    {
        var result = _service.CanReviewVisit(UserRole.Technician, VisitType.BM, ApprovalAction.Approved);

        result.IsAllowed.Should().BeFalse();
    }

    [Fact]
    public void CanReviewVisit_ShouldAllowSupervisorForCmVisit()
    {
        var result = _service.CanReviewVisit(UserRole.Supervisor, VisitType.CM, ApprovalAction.Rejected);

        result.IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void CanReviewVisit_ShouldTreatPreventiveAliasAsBm()
    {
        var result = _service.CanReviewVisit(UserRole.Supervisor, VisitType.BM, ApprovalAction.Approved);

        result.IsAllowed.Should().BeFalse();
    }
}
