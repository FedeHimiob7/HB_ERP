using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Queries.GetPaged
{
    public record GetFiscalTerminalsPagedQuery(
    int PageNumber,
    int PageSize,
    Guid? BranchId = null,
    string? SearchTerm = null
) : IRequest<ErrorOr<PagedFiscalTerminalsResult>>;
}
