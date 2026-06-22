using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Commands.CreateProductSubCategory
{
    internal sealed class CreateProductSubCategoryCommandHandler : IRequestHandler<CreateProductSubCategoryCommand, ErrorOr<Guid>>
    {
        private readonly IProductSubCategoryRepository _repository;
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public CreateProductSubCategoryCommandHandler(
            IProductSubCategoryRepository repository,
            IProductCategoryRepository categoryRepository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateProductSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryId = ProductCategoryId.Create(request.ProductCategoryId);

            // valida existencia Y acceso PSL en un solo query
            if (await _categoryRepository.GetByIdAsync(categoryId, _currentUser.PslIds, cancellationToken) is null)
                return ProductSubCategoryErrors.InvalidCategory;

            if (await _repository.ExistsByNameInCategoryAsync(request.Name, categoryId, cancellationToken: cancellationToken))
                return ProductSubCategoryErrors.DuplicateNameInCategory;

            var result = ProductSubCategory.Create(categoryId, request.Name, request.Description);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
