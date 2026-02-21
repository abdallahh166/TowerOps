namespace TelecomPM.Application.Commands.Visits.StartVisit;

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class StartVisitCommandHandler : IRequestHandler<StartVisitCommand, Result<VisitDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<StartVisitCommandHandler> _logger;

    public StartVisitCommandHandler(
        IVisitRepository visitRepository,
        IChecklistTemplateRepository checklistTemplateRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<StartVisitCommandHandler> logger)
    {
        _visitRepository = visitRepository;
        _checklistTemplateRepository = checklistTemplateRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<VisitDto>> Handle(StartVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitDto>("Visit not found");

        try
        {
            var coordinates = Coordinates.Create(request.Latitude, request.Longitude);
            visit.StartVisit(coordinates);

            var templateVisitType = ResolveChecklistTemplateVisitType(visit.Type);
            var activeTemplate = await _checklistTemplateRepository.GetActiveByVisitTypeAsync(templateVisitType, cancellationToken);
            if (activeTemplate is not null)
            {
                foreach (var templateItem in activeTemplate.Items.OrderBy(i => i.OrderIndex))
                {
                    visit.AddChecklistItem(
                        VisitChecklist.Create(
                            visit.Id,
                            templateItem.Category,
                            templateItem.ItemName,
                            templateItem.Description ?? string.Empty,
                            templateItem.IsMandatory,
                            templateItem.Id));
                }

                visit.ApplyChecklistTemplate(activeTemplate.Id, activeTemplate.Version);
            }
            else
            {
                _logger.LogWarning(
                    "No active checklist template was found for visit {VisitId} and type {VisitType}",
                    visit.Id,
                    templateVisitType);
            }

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<VisitDto>(visit);
            return Result.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result.Failure<VisitDto>(ex.Message);
        }
    }

    private static VisitType ResolveChecklistTemplateVisitType(VisitType visitType)
    {
        return visitType switch
        {
            VisitType.CorrectiveMaintenance => VisitType.CM,
            VisitType.PreventiveMaintenance => VisitType.BM,
            _ => visitType
        };
    }
}
