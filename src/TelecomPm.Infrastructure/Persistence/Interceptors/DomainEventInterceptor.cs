namespace TelecomPM.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Domain.Common;
using TelecomPM.Domain.Interfaces.Services;

/// <summary>
/// Note: Domain events are already dispatched in ApplicationDbContext.SaveChangesAsync.
/// This interceptor is kept for consistency but may not be needed.
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    // Domain events are handled in ApplicationDbContext directly
    // This interceptor is kept for future extensibility
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Domain events are dispatched in ApplicationDbContext.SaveChangesAsync
        // No additional processing needed here
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

