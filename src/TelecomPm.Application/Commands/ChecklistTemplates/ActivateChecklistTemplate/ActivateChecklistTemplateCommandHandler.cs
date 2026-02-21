using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.ChecklistTemplates.ActivateChecklistTemplate;

public class ActivateChecklistTemplateCommandHandler : IRequestHandler<ActivateChecklistTemplateCommand, Result>
{
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateChecklistTemplateCommandHandler(
        IChecklistTemplateRepository checklistTemplateRepository,
        IUnitOfWork unitOfWork)
    {
        _checklistTemplateRepository = checklistTemplateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateChecklistTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _checklistTemplateRepository.GetByIdAsync(request.TemplateId, cancellationToken);
        if (template is null)
            return Result.Failure("Checklist template not found");

        var currentActive = await _checklistTemplateRepository.GetActiveByVisitTypeAsync(template.VisitType, cancellationToken);
        if (currentActive is not null && currentActive.Id != template.Id)
        {
            currentActive.Supersede(DateTime.UtcNow);
            await _checklistTemplateRepository.UpdateAsync(currentActive, cancellationToken);
        }

        try
        {
            template.Activate(request.ApprovedBy);
            await _checklistTemplateRepository.UpdateAsync(template, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to activate checklist template: {ex.Message}");
        }
    }
}
