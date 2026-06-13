using Identity.Application.Common.Models;
using Identity.Domain.DomainErrors;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.SystemActions.Queries.GetSystemActionById
{
    public sealed class GetSystemActionByIdQueryHandler(ISystemActionRepository systemActionRepository)
        : IRequestHandler<GetSystemActionByIdQuery, ErrorOr<SystemActionResponse>>
    {
        public async Task<ErrorOr<SystemActionResponse>> Handle(
            GetSystemActionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var action = await systemActionRepository.GetByIdAsync(new ActionsId(request.Id));
            if (action is null)
                return SystemActionErrors.NotFound;

            return new SystemActionResponse(action.Id.Value, action.Name.Value, action.Description);
        }
    }
}
