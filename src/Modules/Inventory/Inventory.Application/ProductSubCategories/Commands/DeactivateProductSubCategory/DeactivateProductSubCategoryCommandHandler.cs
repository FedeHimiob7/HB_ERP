using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Commands.DeactivateProductSubCategory
{
    internal sealed class DeactivateProductSubCategoryCommandHandler : IRequestHandler<DeactivateProductSubCategoryCommand, ErrorOr<Success>>
    {
        private readonly IProductSubCategoryRepository _repository;
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public DeactivateProductSubCategoryCommandHandler(
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

        public async Task<ErrorOr<Success>> Handle(DeactivateProductSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var subCategory = await _repository.GetByIdAsync(ProductSubCategoryId.Create(request.Id), cancellationToken);
            if (subCategory is null) return ProductSubCategoryErrors.NotFound;

            // verifica acceso PSL a través del padre
            if (await _categoryRepository.GetByIdAsync(subCategory.ProductCategoryId, _currentUser.PslIds, cancellationToken) is null)
                return ProductSubCategoryErrors.NotFound;

            subCategory.Deactivate();

            await _repository.UpdateAsync(subCategory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
