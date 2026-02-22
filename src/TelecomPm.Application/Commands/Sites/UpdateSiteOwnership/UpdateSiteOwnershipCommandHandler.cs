using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Sites.UpdateSiteOwnership;

public sealed class UpdateSiteOwnershipCommandHandler : IRequestHandler<UpdateSiteOwnershipCommand, Result<SiteDetailDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSiteOwnershipCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SiteDetailDto>> Handle(UpdateSiteOwnershipCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetBySiteCodeAsync(request.SiteCode, cancellationToken);
        if (site is null)
            return Result.Failure<SiteDetailDto>("Site not found.");

        site.SetOwnership(
            request.TowerOwnershipType,
            request.TowerOwnerName,
            request.SharingAgreementRef,
            request.HostContactName,
            request.HostContactPhone);

        await _siteRepository.UpdateAsync(site, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<SiteDetailDto>(site));
    }
}
