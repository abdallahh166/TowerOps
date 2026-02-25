namespace TowerOps.Application.Commands.Materials.RestockMaterial;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;

public class RestockMaterialCommandHandler : IRequestHandler<RestockMaterialCommand, Result>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RestockMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestockMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure("Material not found");

        try
        {
            var quantity = MaterialQuantity.Create(request.Quantity, request.Unit);
            
            material.AddStock(quantity, request.RestockedBy);

            if (!string.IsNullOrWhiteSpace(request.Supplier))
            {
                material.SetSupplier(request.Supplier);
            }

            await _materialRepository.UpdateAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Raise MaterialRestockedEvent manually (since AddStock creates Purchase transaction but not the event)
            // Note: Domain event for MaterialRestockedEvent would need to be added in Material.AddStock if not already present

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to restock material: {ex.Message}");
        }
    }
}

