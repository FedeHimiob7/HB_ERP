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
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;
        private readonly IFiscalClock _fiscalClock;

        public UpdateProductPricesCommandHandler(
            IProductRepository repository,
            IFiscalTaxRateRepository fiscalTaxRateRepository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser,
            IFiscalClock fiscalClock)
        {
            _repository = repository;
            _fiscalTaxRateRepository = fiscalTaxRateRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<ProductResponse>> Handle(UpdateProductPricesCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(ProductId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (product is null) return ProductErrors.NotFound;

            var changedByUserId = Guid.TryParse(_currentUser.UserId, out var uid) ? uid : Guid.Empty;

            var newPurchaseTaxIds = request.PurchaseTaxIds ?? new List<Guid>();
            var newSaleTaxIds = request.SaleTaxIds ?? new List<Guid>();

            // Junta los IDs de las 4 listas (compra/venta, viejos/nuevos) en una sola consulta batch,
            // para no golpear la base de datos 4 veces por separado.
            var allTaxIds = newPurchaseTaxIds
                .Concat(newSaleTaxIds)
                .Concat(product.PurchaseTaxIds.Select(t => t.Value))
                .Concat(product.SaleTaxIds.Select(t => t.Value))
                .Distinct()
                .Select(TaxId.Create);

            // Diccionario TaxId -> tasa vigente hoy, para cada impuesto involucrado.
            var effectiveRates = await _fiscalTaxRateRepository.GetEffectiveManyAsync(
                allTaxIds, _fiscalClock.VenezuelaToday, cancellationToken);

            // Suma la tasa vigente de una lista puntual de impuestos (busca cada uno en effectiveRates).
            // Se llama 4 veces abajo, cada vez con una sola categoría (compra o venta) y un solo estado
            // (viejo o nuevo) — nunca mezcla compra con venta.
            decimal SumTaxRates(IEnumerable<Guid> taxIds)
            {
                decimal total = 0m;

                foreach (var taxId in taxIds)
                {
                    if (effectiveRates.TryGetValue(TaxId.Create(taxId), out var fiscalTaxRate))
                    {
                        total += fiscalTaxRate.Rate;
                    }
                }

                return total;
            }

            var oldPurchaseTaxRate = SumTaxRates(product.PurchaseTaxIds.Select(t => t.Value)); // impuestos de compra que tenía el producto antes del update
            var oldSaleTaxRate = SumTaxRates(product.SaleTaxIds.Select(t => t.Value));          // impuestos de venta que tenía el producto antes del update
            var newPurchaseTaxRate = SumTaxRates(newPurchaseTaxIds);                            // impuestos de compra que llegan en el request
            var newSaleTaxRate = SumTaxRates(newSaleTaxIds);                                    // impuestos de venta que llegan en el request

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
