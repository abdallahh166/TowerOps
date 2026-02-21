using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.ChecklistTemplates.CreateChecklistTemplate;

public class CreateChecklistTemplateCommandHandler : IRequestHandler<CreateChecklistTemplateCommand, Result<Guid>>
{
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChecklistTemplateCommandHandler(
        IChecklistTemplateRepository checklistTemplateRepository,
        IUnitOfWork unitOfWork)
    {
        _checklistTemplateRepository = checklistTemplateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateChecklistTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var template = ChecklistTemplate.Create(
                request.VisitType,
                request.Version,
                request.EffectiveFromUtc,
                request.CreatedBy,
                request.ChangeNotes);

            foreach (var item in request.Items.OrderBy(i => i.OrderIndex))
            {
                template.AddItem(
                    item.Category,
                    item.ItemName,
                    item.Description,
                    item.IsMandatory,
                    item.OrderIndex,
                    item.ApplicableSiteTypes,
                    item.ApplicableVisitTypes);
            }

            await _checklistTemplateRepository.AddAsync(template, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(template.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>($"Failed to create checklist template: {ex.Message}");
        }
    }
}
