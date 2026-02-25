namespace TowerOps.Application.Commands.Materials.DeleteMaterial;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;

public class DeleteMaterialCommandHandler : IRequestHandler<DeleteMaterialCommand, Result>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure("Material not found");

        try
        {
            // Soft delete - deactivate instead of deleting
            material.Deactivate();
            material.MarkAsDeleted(request.DeletedBy);
            
            await _materialRepository.UpdateAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to delete material: {ex.Message}");
        }
    }
}

