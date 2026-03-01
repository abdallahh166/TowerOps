using FluentAssertions;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using Xunit;

namespace TowerOps.Domain.Tests.Entities;

public class UserSecurityTests
{
    [Fact]
    public void RegisterFailedLoginAttempt_ThirdLockoutWithinWindow_ShouldRequireManualUnlock()
    {
        var user = User.Create(
            "Security User",
            "security@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());

        var now = DateTime.UtcNow;
        var manualLockTriggered = false;

        for (var lockoutCycle = 0; lockoutCycle < 3; lockoutCycle++)
        {
            for (var attempt = 0; attempt < 5; attempt++)
            {
                manualLockTriggered = user.RegisterFailedLoginAttempt(now);
            }

            if (lockoutCycle < 2)
            {
                manualLockTriggered.Should().BeFalse();
                user.IsLockedOut(now).Should().BeTrue();
                now = now.AddMinutes(16);
            }
        }

        manualLockTriggered.Should().BeTrue();
        user.IsManualLockout.Should().BeTrue();
        user.LockoutEndUtc.Should().BeNull();
    }

    [Fact]
    public void ConfigureMfa_WhenEnabledWithMissingSecret_ShouldThrow()
    {
        var user = User.Create(
            "Security User",
            "security@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());

        var action = () => user.ConfigureMfa(string.Empty, enabled: true);

        action.Should().Throw<TowerOps.Domain.Exceptions.DomainException>();
    }
}

