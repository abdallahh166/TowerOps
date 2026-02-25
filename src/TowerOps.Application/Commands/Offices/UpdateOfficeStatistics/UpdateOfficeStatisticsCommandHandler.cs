namespace TowerOps.Application.Commands.Offices.UpdateOfficeStatistics;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;

public class UpdateOfficeStatisticsCommandHandler : IRequestHandler<UpdateOfficeStatisticsCommand, Result>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOfficeStatisticsCommandHandler(
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork)
    {
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOfficeStatisticsCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure("Office not found");

        try
        {
            office.UpdateStatistics(request.TotalSites, request.ActiveEngineers, request.ActiveTechnicians);
            await _officeRepository.UpdateAsync(office, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to update office statistics: {ex.Message}");
        }
    }
}

