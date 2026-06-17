using ErrorOr;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetAll
{
    internal sealed class GetAllTaxesQueryHandler
        : IRequestHandler<GetAllTaxesQuery, ErrorOr<IReadOnlyList<TaxResponse>>>
    {
        private readonly ITaxRepository _repository;
        public GetAllTaxesQueryHandler(ITaxRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<TaxResponse>>> Handle(
            GetAllTaxesQuery request,
            CancellationToken cancellationToken)
        {
            var taxes = await _repository.GetAllAsync(cancellationToken);

            var response = taxes.Select(t => new TaxResponse(
                t.Id.Value, t.Name, t.TaxType, t.TaxType.ToString(), t.Rate
            )).ToList();

            return response;
        }
    }
}
