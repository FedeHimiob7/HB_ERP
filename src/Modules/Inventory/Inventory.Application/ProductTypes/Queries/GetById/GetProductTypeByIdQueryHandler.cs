using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductTypes.Queries.GetById
{
    internal sealed class GetProductTypeByIdQueryHandler : IRequestHandler<GetProductTypeByIdQuery, ErrorOr<ProductTypeResponse>>
    {
        private readonly IProductTypeRepository _repository;
        public GetProductTypeByIdQueryHandler(IProductTypeRepository repository) => _repository = repository;

        public async Task<ErrorOr<ProductTypeResponse>> Handle(GetProductTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var productType = await _repository.GetByIdAsync(ProductTypeId.Create(request.Id), cancellationToken);
            if (productType is null) return ProductTypeErrors.NotFound;

            return new ProductTypeResponse(productType.Id.Value, productType.Name, productType.Description);
        }
    }
}
