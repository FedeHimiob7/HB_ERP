using ErrorOr;
using MasterData.Application.ExchangeRates.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ExchangeRates.Queries.GetByDate
{
    internal sealed class GetExchangeRateByDateQueryHandler : IRequestHandler<GetExchangeRateByDateQuery, ErrorOr<ExchangeRateResponse>>
    {
        private readonly IExchangeRateRepository _repository;

        public GetExchangeRateByDateQueryHandler(IExchangeRateRepository repository)
            => _repository = repository;

        public async Task<ErrorOr<ExchangeRateResponse>> Handle(GetExchangeRateByDateQuery request, CancellationToken cancellationToken)
        {
            var rate = await _repository.GetLatestByDateAsync(request.Date, cancellationToken);
            if (rate is null) return ExchangeRateErrors.NotFound;

            return new ExchangeRateResponse(
                rate.Id.Value,
                rate.RegisterDate,
                rate.Rate,
                rate.Source,
                rate.Source.ToString());
        }
    }
}
