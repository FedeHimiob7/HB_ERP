using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;
using Identity.Domain.Repositories;

namespace Identity.Application.SystemActions.Queries.GetSystemActionsPaged
{
    public sealed class GetSystemActionsPagedQueryHandler(ISystemActionRepository systemActionRepository)
        : IRequestHandler<GetSystemActionsPagedQuery, ErrorOr<PagedList<SystemActionResponse>>>
    {
        public async Task<ErrorOr<PagedList<SystemActionResponse>>> Handle(
            GetSystemActionsPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (actions, totalCount) = await systemActionRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var items = actions.Select(a => new SystemActionResponse(
                a.Id.Value,
                a.Name.Value,
                a.Description
            )).ToList();

            return new PagedList<SystemActionResponse>(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
