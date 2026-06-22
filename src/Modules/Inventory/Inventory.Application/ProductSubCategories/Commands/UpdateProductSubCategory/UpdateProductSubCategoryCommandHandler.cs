using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Application.ProductSubCategories.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Commands.UpdateProductSubCategory
{
    internal sealed class UpdateProductSubCategoryCommandHandler : IRequestHandler<UpdateProductSubCategoryCommand, ErrorOr<ProductSubCategoryResponse>>
    {
        private readonly IProductSubCategoryRepository _repository;
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateProductSubCategoryCommandHandler(
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

        public async Task<ErrorOr<ProductSubCategoryResponse>> Handle(UpdateProductSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var id = ProductSubCategoryId.Create(request.Id);
            var subCategory = await _repository.GetByIdAsync(id, cancellationToken);
            if (subCategory is null) return ProductSubCategoryErrors.NotFound;

            var categoryId = ProductCategoryId.Create(request.ProductCategoryId);

            // valida existencia de la categoría Y acceso PSL
            if (await _categoryRepository.GetByIdAsync(categoryId, _currentUser.PslIds, cancellationToken) is null)
                return ProductSubCategoryErrors.InvalidCategory;

            if (await _repository.ExistsByNameInCategoryAsync(request.Name, categoryId, excludeId: id, cancellationToken: cancellationToken))
                return ProductSubCategoryErrors.DuplicateNameInCategory;

            var updateResult = subCategory.UpdateDetails(categoryId, request.Name, request.Description);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(subCategory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProductSubCategoryResponse(subCategory.Id.Value, subCategory.ProductCategoryId.Value, subCategory.Name, subCategory.Description);
        }
    }
}
