using ErrorOr;
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
        public GetTaxByIdQueryHandler(ITaxRepository repository) => _repository = repository;

        public async Task<ErrorOr<TaxResponse>> Handle(GetTaxByIdQuery request, CancellationToken cancellationToken)
        {
            var tax = await _repository.GetByIdAsync(TaxId.Create(request.Id), cancellationToken);
            if (tax is null) return TaxErrors.NotFound;

            return new TaxResponse(tax.Id.Value, tax.Name, tax.TaxType, tax.TaxType.ToString(), tax.Rate);
        }
    }
}
