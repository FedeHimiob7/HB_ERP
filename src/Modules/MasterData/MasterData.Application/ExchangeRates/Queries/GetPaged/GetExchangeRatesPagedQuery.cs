using ErrorOr;
using MasterData.Application.ExchangeRates.Models;
using MediatR;

namespace MasterData.Application.ExchangeRates.Queries.GetPaged
{
    public record GetExchangeRatesPagedQuery(int PageNumber, int PageSize) : IRequest<ErrorOr<PagedExchangeRatesResult>>;
}
