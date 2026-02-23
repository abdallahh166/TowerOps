using AutoMapper;
using MediatR;
using TelecomPM.Application.Commands.AuditLogs.LogAuditEntry;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;

public class AcceptByCustomerCommandHandler : IRequestHandler<AcceptByCustomerCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;

    public AcceptByCustomerCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISender sender,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        ISiteRepository siteRepository)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _sender = sender;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _siteRepository = siteRepository;
    }

    public async Task<Result<WorkOrderDto>> Handle(AcceptByCustomerCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder == null)
            return Result.Failure<WorkOrderDto>("Work order not found");

        var ownershipValidation = await ValidatePortalOwnershipAsync(workOrder.SiteCode, cancellationToken);
        if (!ownershipValidation.IsSuccess)
            return Result.Failure<WorkOrderDto>(ownershipValidation.Error);

        try
        {
            var previousState = workOrder.Status.ToString();
            workOrder.AcceptByCustomer(request.AcceptedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _sender.Send(new LogAuditEntryCommand
            {
                EntityType = "WorkOrder",
                EntityId = workOrder.Id,
                Action = "AcceptedByCustomer",
                ActorId = Guid.Empty,
                ActorRole = "Customer",
                PreviousState = previousState,
                NewState = workOrder.Status.ToString(),
                Notes = request.AcceptedBy
            }, cancellationToken);

            return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
        }
        catch (Exception ex)
        {
            return Result.Failure<WorkOrderDto>(ex.Message);
        }
    }

    private async Task<Result> ValidatePortalOwnershipAsync(string siteCode, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return Result.Success();

        var currentUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (currentUser is null || !currentUser.IsClientPortalUser)
            return Result.Success();

        if (string.IsNullOrWhiteSpace(currentUser.ClientCode))
            return Result.Failure("Client portal user is not linked to a client code.");

        var site = await _siteRepository.GetBySiteCodeAsNoTrackingAsync(siteCode, cancellationToken);
        if (site is null)
            return Result.Failure("Site not found.");

        if (!string.Equals(site.ClientCode, currentUser.ClientCode, StringComparison.OrdinalIgnoreCase))
            return Result.Failure("You are not allowed to approve this work order.");

        return Result.Success();
    }
}
