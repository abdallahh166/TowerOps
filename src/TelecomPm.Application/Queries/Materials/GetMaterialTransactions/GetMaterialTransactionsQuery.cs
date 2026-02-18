namespace TelecomPM.Application.Queries.Materials.GetMaterialTransactions;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;

public record GetMaterialTransactionsQuery : IQuery<List<MaterialTransactionDto>>
{
    public Guid MaterialId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

