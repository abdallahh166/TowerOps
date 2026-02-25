using FluentAssertions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Domain.Tests.Entities;

public class SignatureWorkflowTests
{
    private const string ValidPngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO0N7v8AAAAASUVORK5CYII=";

    [Fact]
    public void CaptureClientSignature_ShouldStoreSignatureAndRaiseEvent()
    {
        var workOrder = WorkOrder.Create("WO-SIG-1", "CAI001", "CAI", SlaClass.P2, "Issue");
        var signature = Signature.Create("Client Rep", "ClientRep", ValidPngBase64);

        workOrder.CaptureClientSignature(signature);

        workOrder.IsClientSigned.Should().BeTrue();
        workOrder.ClientSignature.Should().NotBeNull();
        workOrder.DomainEvents.Should().Contain(e => e.GetType().Name == "WorkOrderClientSignedEvent");
    }

    [Fact]
    public void CaptureClientSignature_Twice_ShouldThrow()
    {
        var workOrder = WorkOrder.Create("WO-SIG-2", "CAI001", "CAI", SlaClass.P2, "Issue");
        var signature = Signature.Create("Client Rep", "ClientRep", ValidPngBase64);
        workOrder.CaptureClientSignature(signature);

        var act = () => workOrder.CaptureClientSignature(signature);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CaptureSiteContactSignature_WithLocation_ShouldStoreGps()
    {
        var visit = Visit.Create(
            "V-SIG-1",
            Guid.NewGuid(),
            "CAI001",
            "Site 1",
            Guid.NewGuid(),
            "Engineer",
            DateTime.UtcNow,
            VisitType.BM);

        var location = GeoLocation.Create(30.1234m, 31.5678m);
        var signature = Signature.Create("Site Contact", "SiteContact", ValidPngBase64, signedAtLocation: location);

        visit.CaptureSiteContactSignature(signature);

        visit.IsSiteContactSigned.Should().BeTrue();
        visit.SiteContactSignature.Should().NotBeNull();
        visit.SiteContactSignature!.SignedAtLocation.Should().NotBeNull();
        visit.SiteContactSignature!.SignedAtLocation!.Latitude.Should().Be(30.1234m);
        visit.SiteContactSignature!.SignedAtLocation!.Longitude.Should().Be(31.5678m);
    }
}
