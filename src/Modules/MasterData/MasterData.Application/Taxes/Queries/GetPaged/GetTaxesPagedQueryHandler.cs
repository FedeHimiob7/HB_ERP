using ErrorOr;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetPaged
{
    internal sealed class GetTaxesPagedQueryHandler : IRequestHandler<GetTaxesPagedQuery, ErrorOr<PagedTaxesResult>>
    {
        private readonly ITaxRepository _repository;
        public GetTaxesPagedQueryHandler(ITaxRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedTaxesResult>> Handle(GetTaxesPagedQuery request, CancellationToken cancellationToken)
        {
            var (taxes, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);

            var items = taxes.Select(t => new TaxResponse(
                t.Id.Value, t.Name, t.TaxType, t.TaxType.ToString(), t.Rate
            )).ToList();

            return new PagedTaxesResult(items, totalCount);
        }
    }
}
