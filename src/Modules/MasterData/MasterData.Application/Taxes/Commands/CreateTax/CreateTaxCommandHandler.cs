using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Taxes.Commands.CreateTax
{
    internal sealed class CreateTaxCommandHandler : IRequestHandler<CreateTaxCommand, ErrorOr<Guid>>
    {
        private readonly ITaxRepository _repository;
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;
        private readonly IFiscalClock _fiscalClock;

        public CreateTaxCommandHandler(
            ITaxRepository repository,
            IFiscalTaxRateRepository fiscalTaxRateRepository,
            IMasterDataUnitOfWork unitOfWork,
            IFiscalClock fiscalClock)
        {
            _repository = repository;
            _fiscalTaxRateRepository = fiscalTaxRateRepository;
            _unitOfWork = unitOfWork;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateTaxCommand request, CancellationToken cancellationToken)
        {
            var createResult = Tax.Create(request.Name, request.TaxType);
            if (createResult.IsError) return createResult.Errors;

            var rateResult = FiscalTaxRate.Create(createResult.Value.Id, request.Rate, _fiscalClock.VenezuelaNow);
            if (rateResult.IsError) return rateResult.Errors;

            await _repository.AddAsync(createResult.Value, cancellationToken);
            await _fiscalTaxRateRepository.AddAsync(rateResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return createResult.Value.Id.Value;
        }
    }
}
