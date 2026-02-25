namespace TowerOps.Application.Commands.Materials.UpdateMaterial;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Domain.Interfaces.Repositories;

public class UpdateMaterialCommandHandler : IRequestHandler<UpdateMaterialCommand, Result<MaterialDto>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MaterialDto>> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure<MaterialDto>("Material not found");

        try
        {
            material.UpdateInfo(request.Name, request.Description, request.Category);
            await _materialRepository.UpdateAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<MaterialDto>(material);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<MaterialDto>($"Failed to update material: {ex.Message}");
        }
    }
}

