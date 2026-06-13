namespace Identity.Application.Roles.Commands.DeleteRole
{
    public sealed record DeleteRoleCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;
}
