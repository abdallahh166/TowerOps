namespace TelecomPM.Application.Queries.Reports.GetVisitReport;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.SiteSpecifications;

public class GetVisitReportQueryHandler : IRequestHandler<GetVisitReportQuery, Result<VisitReportDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetVisitReportQueryHandler(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<VisitReportDto>> Handle(GetVisitReportQuery request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitReportDto>("Visit not found");

        var siteSpec = new SiteWithFullDetailsSpecification(visit.SiteId);
        var site = await _siteRepository.FindOneAsync(siteSpec, cancellationToken);
        if (site == null)
            return Result.Failure<VisitReportDto>("Site not found");

        var report = new VisitReportDto
        {
            Visit = _mapper.Map<VisitDetailDto>(visit),
            Site = _mapper.Map<SiteDetailDto>(site),
            PhotoComparisons = GeneratePhotoComparisons(visit),
            TotalMaterialCost = visit.GetTotalMaterialCost().Amount
        };

        return Result.Success(report);
    }

    private List<PhotoComparisonDto> GeneratePhotoComparisons(Visit visit)
    {
        var beforePhotos = visit.Photos.Where(p => p.Type == PhotoType.Before).ToList();
        var afterPhotos = visit.Photos.Where(p => p.Type == PhotoType.After).ToList();

        var comparisons = new List<PhotoComparisonDto>();

        foreach (var before in beforePhotos)
        {
            var after = afterPhotos.FirstOrDefault(a =>
                a.Category == before.Category &&
                a.ItemName == before.ItemName);

            if (after != null)
            {
                comparisons.Add(new PhotoComparisonDto
                {
                    ItemName = before.ItemName,
                    BeforePhotoUrl = before.FilePath,
                    AfterPhotoUrl = after.FilePath,
                    BeforeDescription = before.Description,
                    AfterDescription = after.Description
                });
            }
        }

        return comparisons;
    }
}