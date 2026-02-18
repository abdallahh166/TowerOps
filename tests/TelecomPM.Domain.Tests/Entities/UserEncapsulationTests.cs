using FluentAssertions;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Tests.Entities;

public class UserEncapsulationTests
{
    [Fact]
    public void Specializations_ShouldBeReadOnly()
    {
        var user = User.Create("A", "a@a.com", "010", UserRole.PMEngineer, Guid.NewGuid());
        user.SetEngineerCapacity(3, new List<string> { "Generator Sites" });

        var snapshot = user.Specializations.ToList();
        snapshot.Add("Solar Sites");
        user.Specializations.Should().ContainSingle().And.Contain("Generator Sites");
    }

    [Fact]
    public void AssignedSiteIds_ShouldBeReadOnly()
    {
        var user = User.Create("A", "a@a.com", "010", UserRole.PMEngineer, Guid.NewGuid());
        user.SetEngineerCapacity(1, new List<string>());

        var siteId = Guid.NewGuid();
        user.AssignSite(siteId);

        Action mutate = () => user.AssignedSiteIds.ToList().Add(Guid.NewGuid());
        mutate.Should().NotThrow("copy can be mutated but not the underlying collection");

        user.AssignedSiteIds.Should().ContainSingle().And.Contain(siteId);
    }

    [Fact]
    public void AssignSite_ShouldRespectCapacity()
    {
        var user = User.Create("A", "a@a.com", "010", UserRole.PMEngineer, Guid.NewGuid());
        user.SetEngineerCapacity(1, new List<string>());

        user.AssignSite(Guid.NewGuid());
        Action assignAgain = () => user.AssignSite(Guid.NewGuid());
        assignAgain.Should().Throw<DomainException>();
    }
}


