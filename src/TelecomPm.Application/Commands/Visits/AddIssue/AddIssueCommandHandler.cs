namespace TelecomPM.Application.Commands.Visits.AddIssue;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public class AddIssueCommandHandler : IRequestHandler<AddIssueCommand, Result<VisitIssueDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddIssueCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitIssueDto>> Handle(AddIssueCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitIssueDto>("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure<VisitIssueDto>("Visit cannot be edited");

        try
        {
            var issue = VisitIssue.Create(
                visit.Id,
                request.Category,
                request.Severity,
                request.Title,
                request.Description);

            if (request.PhotoIds != null && request.PhotoIds.Any())
            {
                foreach (var photoId in request.PhotoIds)
                {
                    issue.AttachPhoto(photoId);
                }
            }

            visit.ReportIssue(issue);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<VisitIssueDto>(issue);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<VisitIssueDto>($"Failed to add issue: {ex.Message}");
        }
    }
}

