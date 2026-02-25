
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.DTOs.Materials;

namespace TowerOps.Application.Common.Interfaces;

public interface IExcelExportService
{
    Task<byte[]> ExportVisitToExcelAsync(
        Guid visitId,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportMaterialsToExcelAsync(
        List<MaterialDto> materials,
        CancellationToken cancellationToken = default);
}