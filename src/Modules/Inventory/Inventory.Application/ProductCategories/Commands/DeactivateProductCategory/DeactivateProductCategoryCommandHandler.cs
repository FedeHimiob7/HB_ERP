using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductCategories.Commands.DeactivateProductCategory
{
    internal sealed class DeactivateProductCategoryCommandHandler : IRequestHandler<DeactivateProductCategoryCommand, ErrorOr<Success>>
    {
        private readonly IProductCategoryRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public DeactivateProductCategoryCommandHandler(
            IProductCategoryRepository repository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(ProductCategoryId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (category is null) return ProductCategoryErrors.NotFound;

            category.Deactivate();

            await _repository.UpdateAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
