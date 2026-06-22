using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductTypes.Commands.DeactivateProductType
{
    internal sealed class DeactivateProductTypeCommandHandler : IRequestHandler<DeactivateProductTypeCommand, ErrorOr<Success>>
    {
        private readonly IProductTypeRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public DeactivateProductTypeCommandHandler(IProductTypeRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateProductTypeCommand request, CancellationToken cancellationToken)
        {
            var productType = await _repository.GetByIdAsync(ProductTypeId.Create(request.Id), cancellationToken);
            if (productType is null) return ProductTypeErrors.NotFound;

            productType.Deactivate();

            await _repository.UpdateAsync(productType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
