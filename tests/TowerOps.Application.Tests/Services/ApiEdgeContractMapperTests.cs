using FluentAssertions;
using TowerOps.Api.Contracts.Escalations;
using TowerOps.Api.Contracts.Offices;
using TowerOps.Api.Contracts.WorkOrders;
using TowerOps.Api.Mappings;
using TowerOps.Application.Queries.Reports.GetVisitCompletionTrends;
using TowerOps.Domain.Enums;
using Xunit;

namespace TowerOps.Application.Tests.Services;

public class ApiEdgeContractMapperTests
{
    [Fact]
    public void OfficesMapper_ToCommand_MapsCreateOfficeRequest()
    {
        var request = new CreateOfficeRequest
        {
            Code = "CAI01",
            Name = "Cairo North",
            Region = "Cairo",
            Address = new CreateOfficeAddressRequest { City = "Cairo", Street = "Street 1", Region = "Cairo" },
            Latitude = 30.0,
            Longitude = 31.0,
            ContactPerson = "Manager",
            ContactPhone = "01000",
            ContactEmail = "office@example.com"
        };

        var command = request.ToCommand();

        command.Code.Should().Be(request.Code);
        command.City.Should().Be(request.Address.City);
        command.Street.Should().Be(request.Address.Street);
        command.ContactEmail.Should().Be(request.ContactEmail);
    }

    [Fact]
    public void WorkOrdersMapper_ToAssignCommand_MapsRouteAndBody()
    {
        var workOrderId = Guid.NewGuid();
        var request = new AssignWorkOrderRequest
        {
            EngineerId = Guid.NewGuid(),
            EngineerName = "Eng",
            AssignedBy = "Sup"
        };

        var command = request.ToCommand(workOrderId);

        command.WorkOrderId.Should().Be(workOrderId);
        command.EngineerId.Should().Be(request.EngineerId);
        command.AssignedBy.Should().Be(request.AssignedBy);
    }

    [Fact]
    public void EscalationsMapper_ToCommand_MapsPayload()
    {
        var request = new CreateEscalationRequest
        {
            WorkOrderId = Guid.NewGuid(),
            IncidentId = "INC-1",
            SiteCode = "S-01",
            SlaClass = SlaClass.P1,
            FinancialImpactEgp = 100,
            SlaImpactPercentage = 40,
            EvidencePackage = "Evidence",
            PreviousActions = "Reset",
            RecommendedDecision = "Dispatch",
            Level = EscalationLevel.BMManagement,
            SubmittedBy = "ops"
        };

        var command = request.ToCommand();

        command.WorkOrderId.Should().Be(request.WorkOrderId);
        command.Level.Should().Be(request.Level);
        command.SubmittedBy.Should().Be(request.SubmittedBy);
    }

    [Fact]
    public void AnalyticsMapper_ToVisitCompletionTrendsQuery_UsesDefaults()
    {
        var before = DateTime.UtcNow.AddMonths(-3).AddMinutes(-1);
        var query = AnalyticsContractMapper.ToVisitCompletionTrendsQuery(null, null, null, null, TrendPeriod.Monthly);
        var after = DateTime.UtcNow.AddMinutes(1);

        query.FromDate.Should().BeOnOrAfter(before);
        query.ToDate.Should().BeOnOrBefore(after);
        query.Period.Should().Be(TrendPeriod.Monthly);
    }
}
