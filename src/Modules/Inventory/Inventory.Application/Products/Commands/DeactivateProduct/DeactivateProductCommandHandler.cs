using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Commands.DeactivateProduct
{
    internal sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, ErrorOr<Success>>
    {
        private readonly IProductRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public DeactivateProductCommandHandler(
            IProductRepository repository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(ProductId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (product is null) return ProductErrors.NotFound;

            product.Deactivate();

            await _repository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
