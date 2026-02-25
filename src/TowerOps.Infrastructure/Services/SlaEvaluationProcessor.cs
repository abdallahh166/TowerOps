using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Services;

namespace TowerOps.Infrastructure.Services;

public sealed class SlaEvaluationProcessor
{
    private const int DefaultBatchSize = 200;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ISlaClockService _slaClockService;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SlaEvaluationProcessor> _logger;

    public SlaEvaluationProcessor(
        IWorkOrderRepository workOrderRepository,
        ISlaClockService slaClockService,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork,
        ILogger<SlaEvaluationProcessor> logger)
    {
        _workOrderRepository = workOrderRepository;
        _slaClockService = slaClockService;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> EvaluateBatchAsync(CancellationToken cancellationToken = default)
    {
        var configuredBatchSize = await _settingsService.GetAsync(
            "SLA:Evaluation:BatchSize",
            DefaultBatchSize,
            cancellationToken);

        var batchSize = Math.Clamp(configuredBatchSize, 1, 2_000);
        var workOrders = await _workOrderRepository.GetOpenForSlaEvaluationAsync(batchSize, cancellationToken);
        if (workOrders.Count == 0)
            return 0;

        foreach (var workOrder in workOrders)
        {
            await _slaClockService.EvaluateStatusAsync(workOrder, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "SLA evaluation processed {Count} open work orders in one batch.",
            workOrders.Count);

        return workOrders.Count;
    }
}
