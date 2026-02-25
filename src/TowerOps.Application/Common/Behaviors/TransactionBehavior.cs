namespace TowerOps.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;

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
        // Determine whether the request is a command. Support both non-generic ICommand and generic ICommand<TResponse>.
        var isCommand = request is ICommand
            || request.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));

        if (!isCommand || _unitOfWork.HasActiveTransaction)
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
}