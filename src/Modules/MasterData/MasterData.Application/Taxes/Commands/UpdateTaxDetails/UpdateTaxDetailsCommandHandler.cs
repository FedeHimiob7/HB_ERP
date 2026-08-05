using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Taxes.Commands.UpdateTaxDetails
{
    internal sealed class UpdateTaxDetailsCommandHandler : IRequestHandler<UpdateTaxDetailsCommand, ErrorOr<TaxResponse>>
    {
        private readonly ITaxRepository _repository;
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;
        private readonly IFiscalClock _fiscalClock;

        public UpdateTaxDetailsCommandHandler(
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

        public async Task<ErrorOr<TaxResponse>> Handle(UpdateTaxDetailsCommand request, CancellationToken cancellationToken)
        {
            var tax = await _repository.GetByIdAsync(TaxId.Create(request.Id), cancellationToken);
            if (tax is null) return TaxErrors.NotFound;

            var updateResult = tax.UpdateDetails(request.Name, request.TaxType);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(tax, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var currentRate = await _fiscalTaxRateRepository.GetEffectiveAsync(tax.Id, _fiscalClock.VenezuelaToday, cancellationToken);

            return new TaxResponse(tax.Id.Value, tax.Name, tax.TaxType, tax.TaxType.ToString(), currentRate?.Rate ?? 0m);
        }
    }
}
