using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ExchangeRates.Commands.RegisterExchangeRate
{
    internal sealed class RegisterExchangeRateCommandHandler : IRequestHandler<RegisterExchangeRateCommand, ErrorOr<Guid>>
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;
        private readonly IFiscalClock _fiscalClock;

        public RegisterExchangeRateCommandHandler(IExchangeRateRepository repository, IMasterDataUnitOfWork unitOfWork, IFiscalClock fiscalClock)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<Guid>> Handle(RegisterExchangeRateCommand request, CancellationToken cancellationToken)
        {
            var result = ExchangeRate.Create(request.Rate, request.Source, _fiscalClock.VenezuelaNow);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
