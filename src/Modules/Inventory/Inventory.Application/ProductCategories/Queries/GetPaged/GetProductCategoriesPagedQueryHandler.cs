using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductCategories.Queries.GetPaged
{
    internal sealed class GetProductCategoriesPagedQueryHandler : IRequestHandler<GetProductCategoriesPagedQuery, ErrorOr<PagedProductCategoriesResult>>
    {
        private readonly IProductCategoryRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductCategoriesPagedQueryHandler(IProductCategoryRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<PagedProductCategoriesResult>> Handle(GetProductCategoriesPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, _currentUser.PslIds, cancellationToken);
            var mapped = items.Select(x => new ProductCategoryResponse(
                x.Id.Value,
                x.ProductServiceLineId.Value,
                x.ProductTypeId?.Value,
                x.Name,
                x.Description)).ToList();
            return new PagedProductCategoriesResult(mapped, totalCount);
        }
    }
}
