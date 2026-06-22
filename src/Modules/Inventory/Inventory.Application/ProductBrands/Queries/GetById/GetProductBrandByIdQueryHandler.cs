using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductBrands.Queries.GetById
{
    internal sealed class GetProductBrandByIdQueryHandler : IRequestHandler<GetProductBrandByIdQuery, ErrorOr<ProductBrandResponse>>
    {
        private readonly IProductBrandRepository _repository;
        public GetProductBrandByIdQueryHandler(IProductBrandRepository repository) => _repository = repository;

        public async Task<ErrorOr<ProductBrandResponse>> Handle(GetProductBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var brand = await _repository.GetByIdAsync(ProductBrandId.Create(request.Id), cancellationToken);
            if (brand is null) return ProductBrandErrors.NotFound;

            return new ProductBrandResponse(brand.Id.Value, brand.Name, brand.Description);
        }
    }
}
