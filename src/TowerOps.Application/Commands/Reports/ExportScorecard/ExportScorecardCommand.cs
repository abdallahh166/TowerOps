using FluentValidation;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Commands.Reports.GenerateContractorScorecard;

namespace TowerOps.Application.Commands.Reports.ExportScorecard;

public record ExportScorecardCommand : ICommand<byte[]>
{
    public string OfficeCode { get; init; } = string.Empty;
    public int Month { get; init; }
    public int Year { get; init; }
}

public class ExportScorecardCommandValidator : AbstractValidator<ExportScorecardCommand>
{
    public ExportScorecardCommandValidator()
    {
        RuleFor(x => x.OfficeCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}

public sealed class ExportScorecardCommandHandler : IRequestHandler<ExportScorecardCommand, Result<byte[]>>
{
    private readonly ISender _sender;

    public ExportScorecardCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<Result<byte[]>> Handle(ExportScorecardCommand request, CancellationToken cancellationToken)
    {
        var scorecardResult = await _sender.Send(new GenerateContractorScorecardCommand
        {
            OfficeCode = request.OfficeCode,
            Month = request.Month,
            Year = request.Year
        }, cancellationToken);

        return scorecardResult.IsSuccess && scorecardResult.Value is not null
            ? Result.Success(scorecardResult.Value)
            : Result.Failure<byte[]>(scorecardResult.Error);
    }
}
