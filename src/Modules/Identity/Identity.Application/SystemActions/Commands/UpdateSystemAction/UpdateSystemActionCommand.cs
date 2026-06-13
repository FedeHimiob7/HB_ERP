namespace Identity.Application.SystemActions.Commands.UpdateSystemAction
{
    public sealed record UpdateSystemActionCommand(
        Guid Id,
        string Name,
        string Description) : IRequest<ErrorOr<Updated>>;
}
