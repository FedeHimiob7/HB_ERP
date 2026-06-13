namespace Identity.Application.SystemActions.Commands.DeleteSystemAction
{
    public sealed record DeleteSystemActionCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;
}
