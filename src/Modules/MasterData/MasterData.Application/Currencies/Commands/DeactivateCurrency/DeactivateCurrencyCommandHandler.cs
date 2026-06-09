using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.DeactivateCurrency
{
    internal sealed class DeactivateCurrencyCommandHandler
    : IRequestHandler<DeactivateCurrencyCommand, ErrorOr<Success>>
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateCurrencyCommandHandler(ICurrencyRepository currencyRepository, IMasterDataUnitOfWork unitOfWork)
        {
            _currencyRepository = currencyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currencyId = CurrencyId.Create(request.Id);

            var currency = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);

            if (currency is null)
            {
                return Error.NotFound(
                    code: "Currency.NotFound",
                    description: $"La moneda con el identificador '{request.Id}' no fue encontrada.");
            }

            currency.Deactivate();

            await _currencyRepository.UpdateAsync(currency, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
