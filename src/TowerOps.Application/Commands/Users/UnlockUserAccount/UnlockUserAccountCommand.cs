namespace TowerOps.Application.Commands.Users.UnlockUserAccount;

using TowerOps.Application.Common;

public sealed record UnlockUserAccountCommand : ICommand
{
    public Guid UserId { get; init; }
}

