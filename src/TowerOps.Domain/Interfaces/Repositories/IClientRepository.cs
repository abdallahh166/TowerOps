using TowerOps.Domain.Entities.Clients;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IClientRepository : IRepository<Client, Guid>
{
    Task<Client?> GetByClientCodeAsync(string clientCode, CancellationToken cancellationToken = default);
    Task<Client?> GetByClientCodeAsNoTrackingAsync(string clientCode, CancellationToken cancellationToken = default);
}
