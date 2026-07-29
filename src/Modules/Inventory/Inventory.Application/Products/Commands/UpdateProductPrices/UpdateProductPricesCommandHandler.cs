using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Application.Products.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Commands.UpdateProductPrices
{
    internal sealed class UpdateProductPricesCommandHandler : IRequestHandler<UpdateProductPricesCommand, ErrorOr<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly ITaxRepository _taxRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateProductPricesCommandHandler(
            IProductRepository repository,
            ITaxRepository taxRepository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _taxRepository = taxRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductResponse>> Handle(UpdateProductPricesCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(ProductId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (product is null) return ProductErrors.NotFound;

            var changedByUserId = Guid.TryParse(_currentUser.UserId, out var uid) ? uid : Guid.Empty;

            var newPurchaseTaxIds = request.PurchaseTaxIds ?? new List<Guid>();
            var newSaleTaxIds = request.SaleTaxIds ?? new List<Guid>();

            var allTaxes = await _taxRepository.GetAllAsync(cancellationToken);

            decimal SumRates(IEnumerable<Guid> taxIds)
            {
                var ids = taxIds.ToList();
                return ids.Count == 0
                    ? 0
                    : allTaxes.Where(t => ids.Contains(t.Id.Value)).Sum(t => t.Rate);
            }

            var oldPurchaseTaxRate = SumRates(product.PurchaseTaxIds.Select(t => t.Value));
            var oldSaleTaxRate = SumRates(product.SaleTaxIds.Select(t => t.Value));
            var newPurchaseTaxRate = SumRates(newPurchaseTaxIds);
            var newSaleTaxRate = SumRates(newSaleTaxIds);

            var result = product.UpdatePrices(
                changedByUserId,
                request.Cost,
                request.CostBase,
                request.CostCurrencyId is Guid ccid ? CurrencyId.Create(ccid) : null,
                request.CostExchangeRate,
                request.Price,
                request.PriceBase,
                request.PriceCurrencyId is Guid pcid ? CurrencyId.Create(pcid) : null,
                request.PriceExchangeRate,
                request.Price2,
                request.Price3,
                request.Price4,
                request.Price5,
                newPurchaseTaxIds.Select(id => new TaxId(id)),
                newSaleTaxIds.Select(id => new TaxId(id)),
                oldPurchaseTaxRate,
                newPurchaseTaxRate,
                oldSaleTaxRate,
                newSaleTaxRate,
                request.ProfitMargin);

            if (result.IsError) return result.Errors;

            await _repository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ProductMapper.ToResponse(product);
        }
    }
}
