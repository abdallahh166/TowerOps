namespace TelecomPM.Application.Commands.Sites.DeleteSite;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;

public class DeleteSiteCommandHandler : IRequestHandler<DeleteSiteCommand, Result>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSiteCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure("Site not found");

        if (site.IsDeleted)
            return Result.Failure("Site is already deleted");

        try
        {
            site.MarkAsDeleted(request.DeletedBy);
            await _siteRepository.UpdateAsync(site, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to delete site: {ex.Message}");
        }
    }
}

