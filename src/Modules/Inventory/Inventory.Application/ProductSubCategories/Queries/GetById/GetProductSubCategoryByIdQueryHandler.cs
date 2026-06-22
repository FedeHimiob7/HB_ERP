using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Queries.GetById
{
    internal sealed class GetProductSubCategoryByIdQueryHandler : IRequestHandler<GetProductSubCategoryByIdQuery, ErrorOr<ProductSubCategoryResponse>>
    {
        private readonly IProductSubCategoryRepository _repository;
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductSubCategoryByIdQueryHandler(
            IProductSubCategoryRepository repository,
            IProductCategoryRepository categoryRepository,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductSubCategoryResponse>> Handle(GetProductSubCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var subCategory = await _repository.GetByIdAsync(ProductSubCategoryId.Create(request.Id), cancellationToken);
            if (subCategory is null) return ProductSubCategoryErrors.NotFound;

            // verifica acceso PSL a través del padre
            if (await _categoryRepository.GetByIdAsync(subCategory.ProductCategoryId, _currentUser.PslIds, cancellationToken) is null)
                return ProductSubCategoryErrors.NotFound;

            return new ProductSubCategoryResponse(subCategory.Id.Value, subCategory.ProductCategoryId.Value, subCategory.Name, subCategory.Description);
        }
    }
}
