
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Application.Commands.Visits.SubmitVisit;

public class SubmitVisitCommandHandler : IRequestHandler<SubmitVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IVisitValidationService _validationService;
    private readonly IEvidencePolicyService _evidencePolicyService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVisitCommandHandler(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        IVisitValidationService validationService,
        IEvidencePolicyService evidencePolicyService,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
        _validationService = validationService;
        _evidencePolicyService = evidencePolicyService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        var site = await _siteRepository.GetByIdAsync(visit.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure("Site not found");

        // Validate visit completion
        var validationResult = _validationService.ValidateVisitCompletion(visit, site);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.SelectMany(e => e.Value));
            return Result.Failure($"Visit validation failed: {errors}");
        }

        var evidencePolicy = EvidencePolicy.DefaultFor(visit.Type);
        var evidenceResult = _evidencePolicyService.Validate(visit, evidencePolicy);
        if (!evidenceResult.IsValid)
        {
            var errors = string.Join(", ", evidenceResult.Errors.SelectMany(e => e.Value));
            return Result.Failure($"Evidence policy validation failed: {errors}");
        }

        try
        {
            visit.Submit();

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
