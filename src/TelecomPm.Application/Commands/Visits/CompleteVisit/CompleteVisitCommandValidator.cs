using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelecomPM.Application.Commands.Visits.CompleteVisit;

namespace TelecomPm.Application.Commands.Visits.CompleteVisit
{
    public class CompleteVisitCommandValidator : AbstractValidator<CompleteVisitCommand>
    {
        public CompleteVisitCommandValidator()
        {
            RuleFor(x => x.VisitId)
                .NotEmpty().WithMessage("Visit ID is required");

            RuleFor(x => x.EngineerNotes)
                .MaximumLength(1000)
                .WithMessage("Engineer notes cannot exceed 1000 characters");
        }
    }
}
