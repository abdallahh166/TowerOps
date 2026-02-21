using FluentAssertions;
using TelecomPM.Application.Commands.Auth.ChangePassword;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Auth;

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
