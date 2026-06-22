using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Models;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Queries.GetAll
{
    internal sealed class GetAllProductSubCategoriesQueryHandler : IRequestHandler<GetAllProductSubCategoriesQuery, ErrorOr<IReadOnlyList<ProductSubCategoryResponse>>>
    {
        private readonly IProductSubCategoryRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetAllProductSubCategoriesQueryHandler(IProductSubCategoryRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<IReadOnlyList<ProductSubCategoryResponse>>> Handle(GetAllProductSubCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categoryId = request.ProductCategoryId.HasValue
                ? ProductCategoryId.Create(request.ProductCategoryId.Value)
                : (ProductCategoryId?)null;

            var items = await _repository.GetAllAsync(_currentUser.PslIds, categoryId, cancellationToken);

            return items.Select(x => new ProductSubCategoryResponse(x.Id.Value, x.ProductCategoryId.Value, x.Name, x.Description)).ToList();
        }
    }
}
