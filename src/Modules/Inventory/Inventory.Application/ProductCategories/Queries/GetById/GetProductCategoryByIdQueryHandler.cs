using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductCategories.Queries.GetById
{
    internal sealed class GetProductCategoryByIdQueryHandler : IRequestHandler<GetProductCategoryByIdQuery, ErrorOr<ProductCategoryResponse>>
    {
        private readonly IProductCategoryRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductCategoryByIdQueryHandler(IProductCategoryRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductCategoryResponse>> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(ProductCategoryId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (category is null) return ProductCategoryErrors.NotFound;

            return new ProductCategoryResponse(
                category.Id.Value,
                category.ProductServiceLineId.Value,
                category.ProductTypeId?.Value,
                category.Name,
                category.Description);
        }
    }
}
