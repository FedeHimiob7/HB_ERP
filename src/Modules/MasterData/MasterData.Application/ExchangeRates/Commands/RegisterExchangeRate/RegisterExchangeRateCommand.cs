using ErrorOr;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.ExchangeRates.Commands.RegisterExchangeRate
{
    public record RegisterExchangeRateCommand(decimal Rate, ExchangeRateSource Source) : IRequest<ErrorOr<Guid>>;
}
