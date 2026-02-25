using FluentAssertions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using Xunit;

namespace TowerOps.Domain.Tests.Entities;

public class VisitIssueTests
{
    [Fact]
    public void Create_WithoutTitle_ShouldSetLocalizationKey()
    {
        Action act = () => VisitIssue.Create(
            Guid.NewGuid(),
            IssueCategory.Power,
            IssueSeverity.High,
            string.Empty,
            "desc");

        var ex = act.Should().Throw<DomainException>().Which;
        ex.MessageKey.Should().Be("VisitIssue.Title.Required");
    }

    [Fact]
    public void Close_WhenNotResolved_ShouldSetLocalizationKey()
    {
        var issue = VisitIssue.Create(
            Guid.NewGuid(),
            IssueCategory.Power,
            IssueSeverity.Medium,
            "Title",
            "Desc");

        Action act = issue.Close;

        var ex = act.Should().Throw<DomainException>().Which;
        ex.MessageKey.Should().Be("VisitIssue.Close.RequiresResolved");
    }
}
