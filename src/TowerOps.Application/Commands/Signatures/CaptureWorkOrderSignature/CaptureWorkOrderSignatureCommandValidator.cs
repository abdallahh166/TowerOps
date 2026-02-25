using FluentValidation;

namespace TowerOps.Application.Commands.Signatures.CaptureWorkOrderSignature;

public sealed class CaptureWorkOrderSignatureCommandValidator : AbstractValidator<CaptureWorkOrderSignatureCommand>
{
    public CaptureWorkOrderSignatureCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.SignerName).NotEmpty();
        RuleFor(x => x.SignerRole).NotEmpty();
        RuleFor(x => x.SignatureDataBase64).NotEmpty();
    }
}
