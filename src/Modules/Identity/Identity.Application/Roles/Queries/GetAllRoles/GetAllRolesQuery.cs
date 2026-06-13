using Identity.Application.Common.Models;

namespace Identity.Application.Roles.Queries.GetAllRoles
{
    public sealed record GetAllRolesQuery() : IRequest<ErrorOr<List<RoleSummaryResponse>>>;
}
