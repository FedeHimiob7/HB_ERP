using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Queries.GetById
{
    internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductByIdQueryHandler(IProductRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(ProductId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (product is null) return ProductErrors.NotFound;

            return ProductMapper.ToResponse(product);
        }
    }
}
