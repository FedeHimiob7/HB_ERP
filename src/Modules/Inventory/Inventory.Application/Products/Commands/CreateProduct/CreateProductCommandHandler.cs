using ErrorOr;
using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Commands.CreateProduct
{
    internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ErrorOr<Guid>>
    {
        private readonly IProductRepository _repository;
        private readonly IProductServiceLineRepository _pslRepository;
        private readonly IProductCodeCounterRepository _counterRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public CreateProductCommandHandler(
            IProductRepository repository,
            IProductServiceLineRepository pslRepository,
            IProductCodeCounterRepository counterRepository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _pslRepository = pslRepository;
            _counterRepository = counterRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.PslIds.Contains(request.ProductServiceLineId))
                return CommonErrors.PslAccessDenied;

            var pslId = ProductServiceLineId.Create(request.ProductServiceLineId);

            if (await _pslRepository.GetByIdAsync(pslId, cancellationToken) is null)
                return ProductErrors.InvalidProductServiceLine;

            if (await _repository.ExistsByCodeAsync(request.Code, cancellationToken: cancellationToken))
                return ProductErrors.DuplicateCode;

            // Consumir la reserva generada por GenerateProductCode.
            // Si el usuario editó el código, ConsumeAsync devuelve null y se genera un nuevo correlativo.
            var itemNumberByDay = await _counterRepository.ConsumeAsync(request.Code, pslId, cancellationToken);

            if (itemNumberByDay is null)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var (_, counter, _) = await _counterRepository.ReserveNextAsync(
                    pslId,
                    today,
                    (_, c) => request.Code,
                    cancellationToken);

                // Consumir inmediatamente la reserva recién creada
                itemNumberByDay = await _counterRepository.ConsumeAsync(request.Code, pslId, cancellationToken);
                itemNumberByDay ??= counter;
            }

            var result = Product.Create(
                request.Code,
                itemNumberByDay.Value,
                request.Name,
                pslId,
                request.Cost,
                request.CostCurrencyId is Guid ccid ? CurrencyId.Create(ccid) : null,
                request.CostExchangeRate,
                request.Price,
                request.PriceCurrencyId is Guid pcid ? CurrencyId.Create(pcid) : null,
                request.PriceExchangeRate,
                request.IsSalable,
                request.IsPurchasable,
                request.IsStored);

            if (result.IsError) return result.Errors;

            var product = result.Value;

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

            await _repository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id.Value;
        }
    }
}
