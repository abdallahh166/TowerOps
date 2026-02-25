using FluentAssertions;
using TowerOps.Application.Commands.Auth.ChangePassword;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class ChangePasswordCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldFail_WhenPasswordDoesNotMeetComplexity()
    {
        var validator = new ChangePasswordCommandValidator();

        var result = validator.Validate(new ChangePasswordCommand
        {
            CurrentPassword = "Current1A",
            NewPassword = "weakpass",
            ConfirmPassword = "weakpass"
        });

        result.IsValid.Should().BeFalse();
    }
}
