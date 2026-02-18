using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var existing = await _workOrderRepository.GetByWoNumberAsync(request.WoNumber, cancellationToken);
        if (existing != null)
            return Result.Failure<WorkOrderDto>($"Work order with number {request.WoNumber} already exists");

        var workOrder = WorkOrder.Create(
            request.WoNumber,
            request.SiteCode,
            request.OfficeCode,
            request.SlaClass,
            request.IssueDescription);

        await _workOrderRepository.AddAsync(workOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
    }
}
