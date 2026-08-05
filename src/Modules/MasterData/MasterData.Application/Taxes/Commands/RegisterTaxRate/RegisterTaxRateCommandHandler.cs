using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Taxes.Commands.RegisterTaxRate
{
    internal sealed class RegisterTaxRateCommandHandler : IRequestHandler<RegisterTaxRateCommand, ErrorOr<Guid>>
    {
        private readonly ITaxRepository _taxRepository;
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;
        private readonly IFiscalClock _fiscalClock;

        public RegisterTaxRateCommandHandler(
            ITaxRepository taxRepository,
            IFiscalTaxRateRepository fiscalTaxRateRepository,
            IMasterDataUnitOfWork unitOfWork,
            IFiscalClock fiscalClock)
        {
            _taxRepository = taxRepository;
            _fiscalTaxRateRepository = fiscalTaxRateRepository;
            _unitOfWork = unitOfWork;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<Guid>> Handle(RegisterTaxRateCommand request, CancellationToken cancellationToken)
        {
            var taxId = TaxId.Create(request.TaxId);

            var tax = await _taxRepository.GetByIdAsync(taxId, cancellationToken);
            if (tax is null) return TaxErrors.NotFound;

            var result = FiscalTaxRate.Create(taxId, request.Rate, _fiscalClock.VenezuelaNow);
            if (result.IsError) return result.Errors;

            await _fiscalTaxRateRepository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
