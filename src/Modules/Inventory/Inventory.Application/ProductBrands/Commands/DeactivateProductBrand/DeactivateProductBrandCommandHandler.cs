using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductBrands.Commands.DeactivateProductBrand
{
    internal sealed class DeactivateProductBrandCommandHandler : IRequestHandler<DeactivateProductBrandCommand, ErrorOr<Success>>
    {
        private readonly IProductBrandRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public DeactivateProductBrandCommandHandler(IProductBrandRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateProductBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repository.GetByIdAsync(ProductBrandId.Create(request.Id), cancellationToken);
            if (brand is null) return ProductBrandErrors.NotFound;

            brand.Deactivate();

            await _repository.UpdateAsync(brand, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
