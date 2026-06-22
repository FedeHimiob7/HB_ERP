using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductBrands.Commands.CreateProductBrand
{
    internal sealed class CreateProductBrandCommandHandler : IRequestHandler<CreateProductBrandCommand, ErrorOr<Guid>>
    {
        private readonly IProductBrandRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public CreateProductBrandCommandHandler(IProductBrandRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateProductBrandCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
                return ProductBrandErrors.DuplicateName;

            var result = ProductBrand.Create(request.Name, request.Description);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
