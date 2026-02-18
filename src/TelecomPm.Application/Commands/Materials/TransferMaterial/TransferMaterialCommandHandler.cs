namespace TelecomPM.Application.Commands.Materials.TransferMaterial;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class TransferMaterialCommandHandler : IRequestHandler<TransferMaterialCommand, Result>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(TransferMaterialCommand request, CancellationToken cancellationToken)
    {
        // Get source material
        var sourceMaterial = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (sourceMaterial == null)
            return Result.Failure("Source material not found");

        if (sourceMaterial.OfficeId != request.FromOfficeId)
            return Result.Failure("Source material does not belong to the specified office");

        // Validate target office
        var targetOffice = await _officeRepository.GetByIdAsync(request.ToOfficeId, cancellationToken);
        if (targetOffice == null)
            return Result.Failure("Target office not found");

        try
        {
            var quantity = MaterialQuantity.Create(request.Quantity, request.Unit);

            // Check if source has enough stock
            if (!sourceMaterial.IsStockAvailable(quantity))
                return Result.Failure("Insufficient stock in source office");

            // Transfer from source (creates Transfer transaction)
            sourceMaterial.TransferStock(quantity, $"Transferred to office {targetOffice.Code}: {request.Reason}", request.TransferredBy);

            // Find or create target material
            var targetMaterial = await _materialRepository.GetByCodeAsync(sourceMaterial.Code, cancellationToken);
            
            if (targetMaterial == null || targetMaterial.OfficeId != request.ToOfficeId)
            {
                // Create new material in target office
                var initialStock = quantity;
                var minimumStock = sourceMaterial.MinimumStock;
                var unitCost = sourceMaterial.UnitCost;

                targetMaterial = Material.Create(
                    sourceMaterial.Code,
                    sourceMaterial.Name,
                    sourceMaterial.Description,
                    sourceMaterial.Category,
                    request.ToOfficeId,
                    initialStock,
                    minimumStock,
                    unitCost);

                if (!string.IsNullOrWhiteSpace(sourceMaterial.Supplier))
                {
                    targetMaterial.SetSupplier(sourceMaterial.Supplier);
                }

                await _materialRepository.AddAsync(targetMaterial, cancellationToken);
            }
            else
            {
                // Add to existing material in target office
                targetMaterial.AddStock(quantity);
                await _materialRepository.UpdateAsync(targetMaterial, cancellationToken);
            }

            // Transaction is already recorded in DeductStock method
            await _materialRepository.UpdateAsync(sourceMaterial, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to transfer material: {ex.Message}");
        }
    }
}

