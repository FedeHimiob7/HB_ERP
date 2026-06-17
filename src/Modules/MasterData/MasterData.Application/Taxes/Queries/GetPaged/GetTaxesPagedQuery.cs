using ErrorOr;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.SearchParametersModel;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetPaged
{
    public record GetTaxesPagedQuery(TaxFilter Filter) : IRequest<ErrorOr<PagedTaxesResult>>;
}
