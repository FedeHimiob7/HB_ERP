using ErrorOr;
using MasterData.Application.ExchangeRates.Models;
using MediatR;

namespace MasterData.Application.ExchangeRates.Queries.GetCurrent
{
    public record GetCurrentExchangeRateQuery : IRequest<ErrorOr<ExchangeRateResponse>>;
}
