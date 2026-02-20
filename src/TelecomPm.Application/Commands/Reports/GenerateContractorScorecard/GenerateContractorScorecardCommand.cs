using TelecomPM.Application.Common;

namespace TelecomPM.Application.Commands.Reports.GenerateContractorScorecard;

public record GenerateContractorScorecardCommand : ICommand<byte[]>
{
    public string OfficeCode { get; init; } = string.Empty;
    public int Month { get; init; }
    public int Year { get; init; }
}
