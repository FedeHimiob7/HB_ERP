using ErrorOr;
using MasterData.Application.ExchangeRates.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ExchangeRates.Queries.GetPaged
{
    internal sealed class GetExchangeRatesPagedQueryHandler : IRequestHandler<GetExchangeRatesPagedQuery, ErrorOr<PagedExchangeRatesResult>>
    {
        private readonly IExchangeRateRepository _repository;

        public GetExchangeRatesPagedQueryHandler(IExchangeRateRepository repository)
            => _repository = repository;

        public async Task<ErrorOr<PagedExchangeRatesResult>> Handle(GetExchangeRatesPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var response = items.Select(r => new ExchangeRateResponse(
                r.Id.Value,
                r.RegisterDate,
                r.Rate,
                r.Source,
                r.Source.ToString())).ToList();

            return new PagedExchangeRatesResult(response, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
