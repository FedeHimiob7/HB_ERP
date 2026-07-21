using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Application.Products.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Commands.UpdateProduct
{
    internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateProductCommandHandler(
            IProductRepository repository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(ProductId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (product is null) return ProductErrors.NotFound;

            if (!string.IsNullOrWhiteSpace(request.Code) && request.Code != product.Code)
            {
                if (await _repository.ExistsByCodeAsync(request.Code, excludeId: product.Id, cancellationToken))
                    return ProductErrors.DuplicateCode;

                product.UpdateCode(request.Code);
            }

            var updateResult = product.UpdateDetails(
                request.Name,
                request.Description,
                request.Model,
                request.Barcode,
                request.ClientCode,
                request.ProductTypeId is Guid tid ? new ProductTypeId(tid) : null,
                request.ProductCategoryId is Guid cid ? new ProductCategoryId(cid) : null,
                request.ProductSubCategoryId is Guid sid ? new ProductSubCategoryId(sid) : null,
                request.ProductBrandId is Guid bid ? new ProductBrandId(bid) : null,
                request.IsSalable,
                request.IsPurchasable,
                request.IsStored,
                request.PurchaseUnitId is Guid puid ? new UnitId(puid) : null,
                request.SaleUnitId is Guid suid ? new UnitId(suid) : null,
                request.UnitConversionFactor,
                request.Weight,
                request.Volume,
                request.ContentCapacity,
                request.Tags,
                request.ImageUrl,
                request.ProfitMargin);

            if (updateResult.IsError) return updateResult.Errors;

            product.SetTaxes(
                request.PurchaseTaxIds.Select(id => new TaxId(id)),
                request.SaleTaxIds.Select(id => new TaxId(id)));

            await _repository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ProductMapper.ToResponse(product);
        }
    }
}
