using FluentValidation;

namespace TowerOps.Application.Commands.Users.UnlockUserAccount;

public sealed class UnlockUserAccountCommandValidator : AbstractValidator<UnlockUserAccountCommand>
{
    public UnlockUserAccountCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

