using Identity.Application.Common.Models;
using Identity.Domain.Repositories;

namespace Identity.Application.SystemActions.Queries.GetAllSystemActions
{
    public sealed class GetAllSystemActionsQueryHandler(ISystemActionRepository systemActionRepository)
        : IRequestHandler<GetAllSystemActionsQuery, ErrorOr<List<SystemActionResponse>>>
    {
        public async Task<ErrorOr<List<SystemActionResponse>>> Handle(
            GetAllSystemActionsQuery request,
            CancellationToken cancellationToken)
        {
            var actions = await systemActionRepository.GetAllAsync();

            return actions.Select(a => new SystemActionResponse(
                a.Id.Value,
                a.Name.Value,
                a.Description
            )).ToList();
        }
    }
}
