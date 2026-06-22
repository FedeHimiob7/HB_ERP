using ErrorOr;
using MasterData.Application.ExchangeRates.Models;
using MediatR;

namespace MasterData.Application.ExchangeRates.Queries.GetByDate
{
    public record GetExchangeRateByDateQuery(DateOnly Date) : IRequest<ErrorOr<ExchangeRateResponse>>;
}
