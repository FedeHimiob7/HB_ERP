namespace Identity.Application.Roles.Commands.UpdateRole
{
    public sealed record UpdateRoleCommand(Guid Id, string Name, List<Guid>? ActionIds) : IRequest<ErrorOr<Updated>>;
}
