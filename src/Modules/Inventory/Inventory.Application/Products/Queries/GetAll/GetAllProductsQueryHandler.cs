using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Models;
using Inventory.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Queries.GetAll
{
    internal sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ErrorOr<IReadOnlyList<ProductResponse>>>
    {
        private readonly IProductRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetAllProductsQueryHandler(IProductRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<IReadOnlyList<ProductResponse>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            ProductServiceLineId? pslId = request.ProductServiceLineId is Guid id
                ? ProductServiceLineId.Create(id)
                : null;

            var items = await _repository.GetAllAsync(_currentUser.PslIds, pslId, cancellationToken);
            return items.Select(ProductMapper.ToResponse).ToList();
        }
    }
}
