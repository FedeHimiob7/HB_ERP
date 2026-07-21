using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.Products.Queries.GetPaged
{
    internal sealed class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, ErrorOr<PagedProductsResult>>
    {
        private readonly IProductRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductsPagedQueryHandler(IProductRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<PagedProductsResult>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, _currentUser.PslIds, cancellationToken);
            return new PagedProductsResult(items.Select(ProductMapper.ToResponse).ToList(), totalCount);
        }
    }
}
