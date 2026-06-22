using ErrorOr;
using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.ProductCategories.Commands.CreateProductCategory
{
    internal sealed class CreateProductCategoryCommandHandler : IRequestHandler<CreateProductCategoryCommand, ErrorOr<Guid>>
    {
        private readonly IProductCategoryRepository _repository;
        private readonly IProductServiceLineRepository _pslRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public CreateProductCategoryCommandHandler(
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

        public async Task<ErrorOr<Guid>> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.PslIds.Contains(request.ProductServiceLineId))
                return CommonErrors.PslAccessDenied;

            var pslId = ProductServiceLineId.Create(request.ProductServiceLineId);

            if (await _pslRepository.GetByIdAsync(pslId, cancellationToken) is null)
                return ProductCategoryErrors.InvalidProductServiceLine;

            if (request.ProductTypeId.HasValue)
            {
                var typeId = ProductTypeId.Create(request.ProductTypeId.Value);
                if (await _productTypeRepository.GetByIdAsync(typeId, cancellationToken) is null)
                    return ProductCategoryErrors.InvalidProductType;
            }

            if (await _repository.ExistsByNameInPslAsync(request.Name, pslId, cancellationToken: cancellationToken))
                return ProductCategoryErrors.DuplicateNameInPsl;

            ProductTypeId? productTypeId = request.ProductTypeId is Guid pid ? new ProductTypeId(pid) : null;

            var result = ProductCategory.Create(pslId, productTypeId, request.Name, request.Description);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
