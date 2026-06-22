using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductTypes.Commands.CreateProductType
{
    internal sealed class CreateProductTypeCommandHandler : IRequestHandler<CreateProductTypeCommand, ErrorOr<Guid>>
    {
        private readonly IProductTypeRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public CreateProductTypeCommandHandler(IProductTypeRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateProductTypeCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
                return ProductTypeErrors.DuplicateName;

            var result = ProductType.Create(request.Name, request.Description);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
