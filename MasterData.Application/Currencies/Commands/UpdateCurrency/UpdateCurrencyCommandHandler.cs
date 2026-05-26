using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Application.Currencies.Commands.CreateCurrencie;
using MasterData.Application.Currencies.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.UpdateCurrency
{
    internal sealed class UpdateCurrencyCommandHandler
     : IRequestHandler<UpdateCurrencyCommand, ErrorOr<CurrencyResponse>> // <-- Firma actualizada
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCurrencyCommandHandler> _logger;

        public UpdateCurrencyCommandHandler(ICurrencyRepository currencyRepository, 
                                            IUnitOfWork unitOfWork,
                                            ILogger<CreateCurrencyCommandHandler> logger)
        {
            _currencyRepository = currencyRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ErrorOr<CurrencyResponse>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currencyId = CurrencyId.Create(request.Id);

            var currency = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);

            if (currency is null)
            {
                return Error.NotFound(
                    code: "Currency.NotFound",
                    description: $"La moneda con el identificador '{request.Id}' no fue encontrada.");
            }

            var updateResult = currency.UpdateDetails(request.Name, request.Symbol);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            await _currencyRepository.UpdateAsync(currency, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var finalResponse = new CurrencyResponse(
                currency.Id.Value,
                currency.Code,
                currency.Name,
                currency.Symbol
            );

            return finalResponse;
        }
    }
}
