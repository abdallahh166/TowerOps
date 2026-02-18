namespace TelecomPM.Application.Commands.Materials.ReserveMaterial;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class ReserveMaterialCommandHandler : IRequestHandler<ReserveMaterialCommand, Result>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReserveMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure("Material not found");

        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        try
        {
            var quantity = MaterialQuantity.Create(request.Quantity, request.Unit);
            material.ReserveStock(quantity, request.VisitId);

            await _materialRepository.UpdateAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to reserve material: {ex.Message}");
        }
    }
}

