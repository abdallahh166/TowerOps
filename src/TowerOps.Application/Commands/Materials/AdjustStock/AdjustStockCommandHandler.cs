namespace TowerOps.Application.Commands.Materials.AdjustStock;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, Result>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockCommandHandler(
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure("Material not found");

        try
        {
            var newQuantity = MaterialQuantity.Create(request.NewQuantity, request.Unit);
            material.AdjustStock(newQuantity, request.Reason);

            await _materialRepository.UpdateAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to adjust stock: {ex.Message}");
        }
    }
}

