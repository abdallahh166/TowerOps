using FluentValidation;

namespace TowerOps.Application.Commands.Signatures.CaptureVisitSignature;

public sealed class CaptureVisitSignatureCommandValidator : AbstractValidator<CaptureVisitSignatureCommand>
{
    public CaptureVisitSignatureCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.SignerName).NotEmpty();
        RuleFor(x => x.SignerRole).NotEmpty();
        RuleFor(x => x.SignatureDataBase64).NotEmpty();
    }
}
