using Identity.Application.Common.Models;

namespace Identity.Application.SystemActions.Queries.GetAllSystemActions
{
    public sealed record GetAllSystemActionsQuery() : IRequest<ErrorOr<List<SystemActionResponse>>>;
}
