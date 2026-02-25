namespace TowerOps.Application.Commands.Materials.CreateMaterial;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;

public class CreateMaterialCommandHandler : IRequestHandler<CreateMaterialCommand, Result<MaterialDto>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMaterialCommandHandler(
        IMaterialRepository materialRepository,
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MaterialDto>> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
    {
        // Validate office exists
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<MaterialDto>("Office not found");

        // Check if material code already exists
        var existing = await _materialRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing != null && existing.OfficeId == request.OfficeId)
            return Result.Failure<MaterialDto>($"Material with code {request.Code} already exists in this office");

        try
        {
            var initialStock = MaterialQuantity.Create(request.InitialStock, request.Unit);
            var minimumStock = MaterialQuantity.Create(request.MinimumStock, request.Unit);
            var unitCost = Money.Create(request.UnitCost, request.Currency);

            var material = Material.Create(
                request.Code,
                request.Name,
                request.Description,
                request.Category,
                request.OfficeId,
                initialStock,
                minimumStock,
                unitCost);

            if (!string.IsNullOrWhiteSpace(request.Supplier))
            {
                material.SetSupplier(request.Supplier);
            }

            await _materialRepository.AddAsync(material, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<MaterialDto>(material);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<MaterialDto>($"Failed to create material: {ex.Message}");
        }
    }
}

