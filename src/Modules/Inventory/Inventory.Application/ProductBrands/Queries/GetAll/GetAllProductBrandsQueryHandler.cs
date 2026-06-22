using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductBrands.Queries.GetAll
{
    internal sealed class GetAllProductBrandsQueryHandler : IRequestHandler<GetAllProductBrandsQuery, ErrorOr<IReadOnlyList<ProductBrandResponse>>>
    {
        private readonly IProductBrandRepository _repository;
        public GetAllProductBrandsQueryHandler(IProductBrandRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<ProductBrandResponse>>> Handle(GetAllProductBrandsQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(x => new ProductBrandResponse(x.Id.Value, x.Name, x.Description)).ToList();
        }
    }
}
