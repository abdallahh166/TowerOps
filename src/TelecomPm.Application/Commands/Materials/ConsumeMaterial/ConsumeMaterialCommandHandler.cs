namespace TelecomPM.Application.Commands.Materials.ConsumeMaterial;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;

public class ConsumeMaterialCommandHandler : IRequestHandler<ConsumeMaterialCommand, Result>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConsumeMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConsumeMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure("Material not found");

        try
        {
            material.ConsumeStock(request.VisitId, request.PerformedBy);
            await _materialRepository.UpdateAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to consume material: {ex.Message}");
        }
    }
}

