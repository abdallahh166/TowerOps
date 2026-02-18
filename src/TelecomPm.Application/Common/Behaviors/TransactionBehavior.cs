namespace TelecomPM.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Domain.Interfaces.Repositories;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsCommand(request) || _unitOfWork.HasActiveTransaction)
        {
            return await next();
        }

        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                return await next();
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }

    private static bool IsCommand(TRequest request)
    {
        return typeof(TRequest).Name.EndsWith("Command", StringComparison.Ordinal);
    }
}