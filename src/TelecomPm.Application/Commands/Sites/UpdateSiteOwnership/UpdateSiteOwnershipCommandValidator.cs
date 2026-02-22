using FluentValidation;

namespace TelecomPM.Application.Commands.Sites.UpdateSiteOwnership;

public sealed class UpdateSiteOwnershipCommandValidator : AbstractValidator<UpdateSiteOwnershipCommand>
{
    public UpdateSiteOwnershipCommandValidator()
    {
        RuleFor(x => x.SiteCode).NotEmpty();
        RuleFor(x => x.HostContactPhone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.HostContactPhone));
    }
}
