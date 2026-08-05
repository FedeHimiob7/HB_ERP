using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetPaged
{
    internal sealed class GetTaxesPagedQueryHandler : IRequestHandler<GetTaxesPagedQuery, ErrorOr<PagedTaxesResult>>
    {
        private readonly ITaxRepository _repository;
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository;
        private readonly IFiscalClock _fiscalClock;

        public GetTaxesPagedQueryHandler(ITaxRepository repository, IFiscalTaxRateRepository fiscalTaxRateRepository, IFiscalClock fiscalClock)
        {
            _repository = repository;
            _fiscalTaxRateRepository = fiscalTaxRateRepository;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<PagedTaxesResult>> Handle(GetTaxesPagedQuery request, CancellationToken cancellationToken)
        {
            var (taxes, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);

            var currentRates = await _fiscalTaxRateRepository.GetEffectiveManyAsync(
                taxes.Select(t => t.Id), _fiscalClock.VenezuelaToday, cancellationToken);

            var items = taxes.Select(t => new TaxResponse(
                t.Id.Value, t.Name, t.TaxType, t.TaxType.ToString(),
                currentRates.TryGetValue(t.Id, out var rate) ? rate.Rate : 0m
            )).ToList();

            return new PagedTaxesResult(items, totalCount);
        }
    }
}
