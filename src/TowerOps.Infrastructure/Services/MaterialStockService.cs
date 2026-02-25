using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using TowerOps.Domain.Services;

namespace TowerOps.Infrastructure.Services;

public sealed class MaterialStockService : IMaterialStockService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MaterialStockService(
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsStockAvailableAsync(
        Guid materialId,
        MaterialQuantity requestedQuantity,
        CancellationToken cancellationToken = default)
    {
        // Use AsNoTracking since we're only checking availability
        var material = await _materialRepository.GetByIdAsNoTrackingAsync(materialId, cancellationToken);

        if (material == null)
            return false;

        return material.IsStockAvailable(requestedQuantity);
    }

    public async Task ReserveMaterialAsync(
        Material material,
        MaterialQuantity quantity,
        Guid visitId,
        CancellationToken cancellationToken = default)
    {
        // Reserve stock in domain
        material.ReserveStock(quantity, visitId);

        // Update the entity (use await)
        await _materialRepository.UpdateAsync(material, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ConsumeMaterialAsync(
        Material material,
        MaterialQuantity quantity,
        Guid visitId,
        string performedBy,
        CancellationToken cancellationToken = default)
    {
        // Consume the reserved stock
        material.ConsumeStock(visitId, performedBy);

        // Update the entity (use await)
        await _materialRepository.UpdateAsync(material, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Material>> GetLowStockMaterialsAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        // Use AsNoTracking for read-only operations
        var materials = await _materialRepository.GetLowStockItemsAsNoTrackingAsync(officeId, cancellationToken);
        return materials.ToList();
    }
}