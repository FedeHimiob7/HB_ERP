using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Models;
using Inventory.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductCategories.Queries.GetAll
{
    internal sealed class GetAllProductCategoriesQueryHandler : IRequestHandler<GetAllProductCategoriesQuery, ErrorOr<IReadOnlyList<ProductCategoryResponse>>>
    {
        private readonly IProductCategoryRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetAllProductCategoriesQueryHandler(IProductCategoryRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<IReadOnlyList<ProductCategoryResponse>>> Handle(GetAllProductCategoriesQuery request, CancellationToken cancellationToken)
        {
            var pslId = request.ProductServiceLineId.HasValue
                ? ProductServiceLineId.Create(request.ProductServiceLineId.Value)
                : (ProductServiceLineId?)null;

            var items = await _repository.GetAllAsync(_currentUser.PslIds, pslId, cancellationToken);

            return items.Select(x => new ProductCategoryResponse(
                x.Id.Value,
                x.ProductServiceLineId.Value,
                x.ProductTypeId?.Value,
                x.Name,
                x.Description)).ToList();
        }
    }
}
