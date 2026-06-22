using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductBrands.Queries.GetPaged
{
    internal sealed class GetProductBrandsPagedQueryHandler : IRequestHandler<GetProductBrandsPagedQuery, ErrorOr<PagedProductBrandsResult>>
    {
        private readonly IProductBrandRepository _repository;
        public GetProductBrandsPagedQueryHandler(IProductBrandRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedProductBrandsResult>> Handle(GetProductBrandsPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);
            var mapped = items.Select(x => new ProductBrandResponse(x.Id.Value, x.Name, x.Description)).ToList();
            return new PagedProductBrandsResult(mapped, totalCount);
        }
    }
}
