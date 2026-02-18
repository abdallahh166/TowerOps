using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Services;

public interface IMaterialStockService
{
    Task<bool> IsStockAvailableAsync(Guid materialId, MaterialQuantity requestedQuantity, CancellationToken cancellationToken = default);
    Task ReserveMaterialAsync(Material material, MaterialQuantity quantity, Guid visitId, CancellationToken cancellationToken = default);
    Task ConsumeMaterialAsync(Material material, MaterialQuantity quantity, Guid visitId, string performedBy, CancellationToken cancellationToken = default);
    Task<List<Material>> GetLowStockMaterialsAsync(Guid officeId, CancellationToken cancellationToken = default);
}
