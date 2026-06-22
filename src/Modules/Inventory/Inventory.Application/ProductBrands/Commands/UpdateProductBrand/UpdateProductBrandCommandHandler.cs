using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Application.ProductBrands.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductBrands.Commands.UpdateProductBrand
{
    internal sealed class UpdateProductBrandCommandHandler : IRequestHandler<UpdateProductBrandCommand, ErrorOr<ProductBrandResponse>>
    {
        private readonly IProductBrandRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public UpdateProductBrandCommandHandler(IProductBrandRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<ProductBrandResponse>> Handle(UpdateProductBrandCommand request, CancellationToken cancellationToken)
        {
            var id = ProductBrandId.Create(request.Id);
            var brand = await _repository.GetByIdAsync(id, cancellationToken);
            if (brand is null) return ProductBrandErrors.NotFound;

            if (await _repository.ExistsByNameAsync(request.Name, excludeId: id, cancellationToken: cancellationToken))
                return ProductBrandErrors.DuplicateName;

            var updateResult = brand.UpdateDetails(request.Name, request.Description);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(brand, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProductBrandResponse(brand.Id.Value, brand.Name, brand.Description);
        }
    }
}
