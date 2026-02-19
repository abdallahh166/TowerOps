namespace TelecomPM.Application.Commands.Visits.AddIssue;

using AutoMapper;
using MediatR;
using System.Linq;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Visits;

public class AddIssueCommandHandler : IRequestHandler<AddIssueCommand, Result<VisitIssueDto>>
{
    private readonly IEditableVisitMutationService _editableVisitMutationService;
    private readonly IMapper _mapper;

    public AddIssueCommandHandler(IEditableVisitMutationService editableVisitMutationService, IMapper mapper)
    {
        _editableVisitMutationService = editableVisitMutationService;
        _mapper = mapper;
    }

    public Task<Result<VisitIssueDto>> Handle(AddIssueCommand request, CancellationToken cancellationToken)
        => _editableVisitMutationService.ExecuteAsync(
            request.VisitId,
            visit =>
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

                var dto = _mapper.Map<VisitIssueDto>(issue);
                return Task.FromResult(dto);
            },
            "Failed to add issue",
            cancellationToken);
}
