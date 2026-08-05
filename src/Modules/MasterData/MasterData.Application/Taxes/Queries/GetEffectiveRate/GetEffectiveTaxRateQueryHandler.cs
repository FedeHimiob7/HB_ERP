using ErrorOr;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetEffectiveRate
{
    internal sealed class GetEffectiveTaxRateQueryHandler : IRequestHandler<GetEffectiveTaxRateQuery, ErrorOr<FiscalTaxRateResponse>>
    {
        private readonly IFiscalTaxRateRepository _repository;

        public GetEffectiveTaxRateQueryHandler(IFiscalTaxRateRepository repository)
            => _repository = repository;

        public async Task<ErrorOr<FiscalTaxRateResponse>> Handle(GetEffectiveTaxRateQuery request, CancellationToken cancellationToken)
        {
            var fiscalTaxRate = await _repository.GetEffectiveAsync(TaxId.Create(request.TaxId), request.AsOfDate, cancellationToken);
            if (fiscalTaxRate is null) return FiscalTaxRateErrors.NotFound;

            return new FiscalTaxRateResponse(fiscalTaxRate.TaxId.Value, fiscalTaxRate.Rate, fiscalTaxRate.EffectiveFrom);
        }
    }
}
