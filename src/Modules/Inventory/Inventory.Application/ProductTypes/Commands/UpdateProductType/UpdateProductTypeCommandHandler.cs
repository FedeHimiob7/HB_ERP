using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Application.ProductTypes.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductTypes.Commands.UpdateProductType
{
    internal sealed class UpdateProductTypeCommandHandler : IRequestHandler<UpdateProductTypeCommand, ErrorOr<ProductTypeResponse>>
    {
        private readonly IProductTypeRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public UpdateProductTypeCommandHandler(IProductTypeRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<ProductTypeResponse>> Handle(UpdateProductTypeCommand request, CancellationToken cancellationToken)
        {
            var id = ProductTypeId.Create(request.Id);
            var productType = await _repository.GetByIdAsync(id, cancellationToken);
            if (productType is null) return ProductTypeErrors.NotFound;

            if (await _repository.ExistsByNameAsync(request.Name, excludeId: id, cancellationToken: cancellationToken))
                return ProductTypeErrors.DuplicateName;

            var updateResult = productType.UpdateDetails(request.Name, request.Description);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(productType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProductTypeResponse(productType.Id.Value, productType.Name, productType.Description);
        }
    }
}
