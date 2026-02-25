using FluentValidation;

namespace TowerOps.Application.Commands.Sync.ProcessSyncBatch;

public sealed class ProcessSyncBatchCommandValidator : AbstractValidator<ProcessSyncBatchCommand>
{
    public ProcessSyncBatchCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.Items).NotNull();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.OperationType).NotEmpty();
            item.RuleFor(i => i.CreatedOnDeviceUtc).NotEqual(default(DateTime));
        });
    }
}
