using ErrorOr;
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

        public RegisterExchangeRateCommandHandler(IExchangeRateRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(RegisterExchangeRateCommand request, CancellationToken cancellationToken)
        {
            var result = ExchangeRate.Create(request.Rate, request.Source);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
