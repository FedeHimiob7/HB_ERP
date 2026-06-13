using Identity.Application.Common.Models;

namespace Identity.Application.SystemActions.Queries.GetSystemActionById
{
    public sealed record GetSystemActionByIdQuery(Guid Id) : IRequest<ErrorOr<SystemActionResponse>>;
}
