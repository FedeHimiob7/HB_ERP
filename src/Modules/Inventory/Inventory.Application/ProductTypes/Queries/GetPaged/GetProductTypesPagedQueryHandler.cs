using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductTypes.Queries.GetPaged
{
    internal sealed class GetProductTypesPagedQueryHandler : IRequestHandler<GetProductTypesPagedQuery, ErrorOr<PagedProductTypesResult>>
    {
        private readonly IProductTypeRepository _repository;
        public GetProductTypesPagedQueryHandler(IProductTypeRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedProductTypesResult>> Handle(GetProductTypesPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);
            var mapped = items.Select(x => new ProductTypeResponse(x.Id.Value, x.Name, x.Description)).ToList();
            return new PagedProductTypesResult(mapped, totalCount);
        }
    }
}
