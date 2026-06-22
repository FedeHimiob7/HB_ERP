using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Application.ProductCategories.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductCategories.Commands.UpdateProductCategory
{
    internal sealed class UpdateProductCategoryCommandHandler : IRequestHandler<UpdateProductCategoryCommand, ErrorOr<ProductCategoryResponse>>
    {
        private readonly IProductCategoryRepository _repository;
        private readonly IProductServiceLineRepository _pslRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateProductCategoryCommandHandler(
            IProductCategoryRepository repository,
            IProductServiceLineRepository pslRepository,
            IProductTypeRepository productTypeRepository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _pslRepository = pslRepository;
            _productTypeRepository = productTypeRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductCategoryResponse>> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var id = ProductCategoryId.Create(request.Id);
            var category = await _repository.GetByIdAsync(id, _currentUser.PslIds, cancellationToken);
            if (category is null) return ProductCategoryErrors.NotFound;

            var pslId = ProductServiceLineId.Create(request.ProductServiceLineId);
            if (await _pslRepository.GetByIdAsync(pslId, cancellationToken) is null)
                return ProductCategoryErrors.InvalidProductServiceLine;

            if (request.ProductTypeId.HasValue)
            {
                var typeId = ProductTypeId.Create(request.ProductTypeId.Value);
                if (await _productTypeRepository.GetByIdAsync(typeId, cancellationToken) is null)
                    return ProductCategoryErrors.InvalidProductType;
            }

            if (await _repository.ExistsByNameInPslAsync(request.Name, pslId, excludeId: id, cancellationToken: cancellationToken))
                return ProductCategoryErrors.DuplicateNameInPsl;

            ProductTypeId? productTypeId = request.ProductTypeId is Guid pid ? new ProductTypeId(pid) : null;

            var updateResult = category.UpdateDetails(pslId, productTypeId, request.Name, request.Description);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProductCategoryResponse(
                category.Id.Value,
                category.ProductServiceLineId.Value,
                category.ProductTypeId?.Value,
                category.Name,
                category.Description);
        }
    }
}
