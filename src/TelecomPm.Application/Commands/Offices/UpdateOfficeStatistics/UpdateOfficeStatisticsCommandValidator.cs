namespace TelecomPM.Application.Commands.Offices.UpdateOfficeStatistics;

using FluentValidation;
using System;

public class UpdateOfficeStatisticsCommandValidator : AbstractValidator<UpdateOfficeStatisticsCommand>
{
    public UpdateOfficeStatisticsCommandValidator()
    {
        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");

        RuleFor(x => x.TotalSites)
            .GreaterThanOrEqualTo(0).WithMessage("Total sites must be zero or greater");

        RuleFor(x => x.ActiveEngineers)
            .GreaterThanOrEqualTo(0).WithMessage("Active engineers must be zero or greater");

        RuleFor(x => x.ActiveTechnicians)
            .GreaterThanOrEqualTo(0).WithMessage("Active technicians must be zero or greater");
    }
}

