namespace TowerOps.Application.Commands.Offices.DeleteOffice;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;

public class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand, Result>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IUserRepository userRepository,
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork)
    {
        _officeRepository = officeRepository;
        _userRepository = userRepository;
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure("Office not found");

        // Check if office has associated users
        var users = await _userRepository.GetByOfficeIdAsync(request.OfficeId, cancellationToken);
        if (users.Any(u => u.IsActive))
            return Result.Failure("Cannot delete office with active users. Please reassign or deactivate users first");

        // Check if office has associated sites
        var sites = await _siteRepository.GetByOfficeIdAsync(request.OfficeId, cancellationToken);
        if (sites.Any(s => !s.IsDeleted))
            return Result.Failure("Cannot delete office with associated sites. Please reassign sites first");

        try
        {
            // Soft delete - deactivate instead of deleting
            office.Deactivate();
            office.MarkAsDeleted(request.DeletedBy);
            
            await _officeRepository.UpdateAsync(office, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to delete office: {ex.Message}");
        }
    }
}

