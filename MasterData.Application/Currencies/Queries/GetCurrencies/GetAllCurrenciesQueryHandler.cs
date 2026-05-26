using ErrorOr;
using MasterData.Application.Currencies.Models;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Queries.GetCurrencies
{
    internal sealed class GetAllCurrenciesQueryHandler
    : IRequestHandler<GetAllCurrenciesQuery, ErrorOr<IReadOnlyList<CurrencyResponse>>>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public GetAllCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<ErrorOr<IReadOnlyList<CurrencyResponse>>> Handle(
            GetAllCurrenciesQuery request,
            CancellationToken cancellationToken)
        {
            
            var currencies = await _currencyRepository.GetAllAsync(cancellationToken);

            var response = currencies.Select(c => new CurrencyResponse(
                c.Id.Value,
                c.Code,
                c.Name,
                c.Symbol
            )).ToList();

            return response; 
        }
    }
}
