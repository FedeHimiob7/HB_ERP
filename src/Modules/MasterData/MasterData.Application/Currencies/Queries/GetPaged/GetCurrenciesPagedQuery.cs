using ErrorOr;
using MasterData.Application.Currencies.Models;
using MasterData.Domain.SearchParametersModel;
using MediatR;

namespace MasterData.Application.Currencies.Queries.GetPaged
{
    public record GetCurrenciesPagedQuery(CurrencyFilter Filter) : IRequest<ErrorOr<PagedCurrenciesResult>>;
}
