using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Queries.GetPaged
{
    internal sealed class GetProductSubCategoriesPagedQueryHandler : IRequestHandler<GetProductSubCategoriesPagedQuery, ErrorOr<PagedProductSubCategoriesResult>>
    {
        private readonly IProductSubCategoryRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductSubCategoriesPagedQueryHandler(IProductSubCategoryRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<PagedProductSubCategoriesResult>> Handle(GetProductSubCategoriesPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, _currentUser.PslIds, cancellationToken);
            var mapped = items.Select(x => new ProductSubCategoryResponse(x.Id.Value, x.ProductCategoryId.Value, x.Name, x.Description)).ToList();
            return new PagedProductSubCategoriesResult(mapped, totalCount);
        }
    }
}
