namespace TowerOps.Application.Commands.Sites.UpdateSiteStatus;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;

public class UpdateSiteStatusCommandHandler : IRequestHandler<UpdateSiteStatusCommand, Result<SiteDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSiteStatusCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SiteDto>> Handle(UpdateSiteStatusCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure<SiteDto>("Site not found");

        try
        {
            site.UpdateStatus(request.Status);
            await _siteRepository.UpdateAsync(site, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<SiteDto>(site);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<SiteDto>($"Failed to update site status: {ex.Message}");
        }
    }
}

