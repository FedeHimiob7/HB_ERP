using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetById
{
    internal sealed class GetTaxByIdQueryHandler : IRequestHandler<GetTaxByIdQuery, ErrorOr<TaxResponse>>
    {
        private readonly ITaxRepository _repository;
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository;
        private readonly IFiscalClock _fiscalClock;

        public GetTaxByIdQueryHandler(ITaxRepository repository, IFiscalTaxRateRepository fiscalTaxRateRepository, IFiscalClock fiscalClock)
        {
            _repository = repository;
            _fiscalTaxRateRepository = fiscalTaxRateRepository;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<TaxResponse>> Handle(GetTaxByIdQuery request, CancellationToken cancellationToken)
        {
            var tax = await _repository.GetByIdAsync(TaxId.Create(request.Id), cancellationToken);
            if (tax is null) return TaxErrors.NotFound;

            var currentRate = await _fiscalTaxRateRepository.GetEffectiveAsync(tax.Id, _fiscalClock.VenezuelaToday, cancellationToken);

            return new TaxResponse(tax.Id.Value, tax.Name, tax.TaxType, tax.TaxType.ToString(), currentRate?.Rate ?? 0m);
        }
    }
}
