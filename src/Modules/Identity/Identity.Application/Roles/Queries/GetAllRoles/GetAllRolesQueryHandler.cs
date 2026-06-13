using Identity.Application.Common.Models;
using Identity.Domain.Repositories;

namespace Identity.Application.Roles.Queries.GetAllRoles
{
    public sealed class GetAllRolesQueryHandler(IRoleRepository roleRepository)
        : IRequestHandler<GetAllRolesQuery, ErrorOr<List<RoleSummaryResponse>>>
    {
        public async Task<ErrorOr<List<RoleSummaryResponse>>> Handle(
            GetAllRolesQuery request,
            CancellationToken cancellationToken)
        {
            var roles = await roleRepository.GetAllAsync();

            return roles.Select(r => new RoleSummaryResponse(
                r.Id.Value,
                r.Name
            )).ToList();
        }
    }
}
