namespace TelecomPM.Application.Commands.Sites.UpdateSite;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Interfaces.Repositories;

public class UpdateSiteCommandHandler : IRequestHandler<UpdateSiteCommand, Result<SiteDetailDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSiteCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SiteDetailDto>> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure<SiteDetailDto>("Site not found");

        try
        {
            site.UpdateBasicInfo(request.Name, request.OMCName, request.SiteType);

            if (!string.IsNullOrWhiteSpace(request.BSCName) && !string.IsNullOrWhiteSpace(request.BSCCode))
            {
                site.SetBSCInfo(request.BSCName, request.BSCCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Subcontractor))
            {
                site.SetContractorInfo(
                    request.Subcontractor,
                    request.MaintenanceArea ?? string.Empty);
            }

            await _siteRepository.UpdateAsync(site, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<SiteDetailDto>(site);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<SiteDetailDto>($"Failed to update site: {ex.Message}");
        }
    }
}

