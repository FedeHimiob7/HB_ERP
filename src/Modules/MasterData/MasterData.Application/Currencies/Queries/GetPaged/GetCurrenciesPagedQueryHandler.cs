using ErrorOr;
using MasterData.Application.Currencies.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Currencies.Queries.GetPaged
{
    internal sealed class GetCurrenciesPagedQueryHandler : IRequestHandler<GetCurrenciesPagedQuery, ErrorOr<PagedCurrenciesResult>>
    {
        private readonly ICurrencyRepository _repository;
        public GetCurrenciesPagedQueryHandler(ICurrencyRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedCurrenciesResult>> Handle(GetCurrenciesPagedQuery request, CancellationToken cancellationToken)
        {
            var (currencies, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);

            var mappedItems = currencies.Select(c => new CurrencyResponse(
                c.Id.Value, c.Code, c.Name, c.Symbol
            )).ToList();

            return new PagedCurrenciesResult(mappedItems, totalCount);
        }
    }
}
