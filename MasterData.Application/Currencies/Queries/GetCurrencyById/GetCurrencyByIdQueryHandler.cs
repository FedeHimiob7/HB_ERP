using ErrorOr;
using MasterData.Application.Currencies.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Queries.GetCurrencyById
{
    internal sealed class GetCurrencyByIdQueryHandler
    : IRequestHandler<GetCurrencyByIdQuery, ErrorOr<CurrencyResponse>>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public GetCurrencyByIdQueryHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<ErrorOr<CurrencyResponse>> Handle(
            GetCurrencyByIdQuery request,
            CancellationToken cancellationToken)
        {
            var currencyId = CurrencyId.Create(request.Id);

            var currency = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);

            if (currency is null)
            {
                return Error.NotFound(
                    code: "Currency.NotFound",
                    description: $"La moneda con el identificador '{request.Id}' no existe.");
            }

            // 4. Retornamos el DTO
            return new CurrencyResponse(
                currency.Id.Value,
                currency.Code,
                currency.Name,
                currency.Symbol
            );
        }
    }
}
