using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.Clients;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Infrastructure.Persistence.Repositories;

public sealed class ClientRepository : Repository<Client, Guid>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Client?> GetByClientCodeAsync(string clientCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ClientCode == clientCode, cancellationToken);
    }

    public async Task<Client?> GetByClientCodeAsNoTrackingAsync(string clientCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.ClientCode == clientCode, cancellationToken);
    }
}
