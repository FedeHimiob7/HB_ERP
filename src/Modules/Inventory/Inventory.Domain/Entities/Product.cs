using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class Product : AggregateRoot<ProductId>
    {
        private readonly List<TaxId> _purchaseTaxIds = new();
        private readonly List<TaxId> _saleTaxIds = new();
        private readonly List<ProductPriceHistory> _priceHistory = new();

        private Product() { }

        private Product(
            ProductId id,
            string code,
            int itemNumberByDay,
            string name,
            ProductServiceLineId productServiceLineId,
            decimal? cost,
            CurrencyId? costCurrencyId,
            decimal? costExchangeRate,
            decimal? price,
            CurrencyId? priceCurrencyId,
            decimal? priceExchangeRate,
            bool isSalable,
            bool isPurchasable,
            bool isStored)
            : base(id)
        {
            Code = code;
            ItemNumberByDay = itemNumberByDay;
            Name = name;
            ProductServiceLineId = productServiceLineId;
            Cost = cost;
            CostCurrencyId = costCurrencyId;
            CostExchangeRate = costExchangeRate;
            Price = price;
            PriceCurrencyId = priceCurrencyId;
            PriceExchangeRate = priceExchangeRate;
            IsSalable = isSalable;
            IsPurchasable = isPurchasable;
            IsStored = isStored;
            IsActive = true;
        }

        // Identidad
        public string Code { get; private set; } = string.Empty;
        public int ItemNumberByDay { get; private set; }
        public string? Barcode { get; private set; }
        public string? ClientCode { get; private set; }

        // Básico
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Model { get; private set; }

        // Clasificación
        public ProductServiceLineId ProductServiceLineId { get; private set; }
        public ProductTypeId? ProductTypeId { get; private set; }
        public ProductCategoryId? ProductCategoryId { get; private set; }
        public ProductSubCategoryId? ProductSubCategoryId { get; private set; }
        public ProductBrandId? ProductBrandId { get; private set; }

        // Flags
        public bool IsSalable { get; private set; }
        public bool IsPurchasable { get; private set; }
        public bool IsStored { get; private set; }

        // Costo
        public decimal? Cost { get; private set; }
        public decimal? CostBase { get; private set; }
        public CurrencyId? CostCurrencyId { get; private set; }
        public decimal? CostExchangeRate { get; private set; }

        // Precio principal (Price1) — define moneda y tasa para todos los precios
        public decimal? Price { get; private set; }
        public decimal? PriceBase { get; private set; }
        public CurrencyId? PriceCurrencyId { get; private set; }
        public decimal? PriceExchangeRate { get; private set; }

        // Precios adicionales (heredan PriceCurrencyId y PriceExchangeRate)
        public decimal? Price2 { get; private set; }
        public decimal? Price3 { get; private set; }
        public decimal? Price4 { get; private set; }
        public decimal? Price5 { get; private set; }

        // Impuestos
        public IReadOnlyList<TaxId> PurchaseTaxIds => _purchaseTaxIds.AsReadOnly();
        public IReadOnlyList<TaxId> SaleTaxIds => _saleTaxIds.AsReadOnly();

        // Unidades
        public UnitId? PurchaseUnitId { get; private set; }
        public UnitId? SaleUnitId { get; private set; }
        public decimal? UnitConversionFactor { get; private set; }

        // Físico / empaque
        public decimal? Weight { get; private set; }
        public decimal? Volume { get; private set; }
        public decimal? ContentCapacity { get; private set; }

        // Misc
        public string? Tags { get; private set; }
        public string? ImageUrl { get; private set; }
        public decimal? ProfitMargin { get; private set; }

        // Historial
        public IReadOnlyList<ProductPriceHistory> PriceHistory => _priceHistory.AsReadOnly();

        public bool IsActive { get; private set; }

        public static ErrorOr<Product> Create(
            string code,
            int itemNumberByDay,
            string name,
            ProductServiceLineId productServiceLineId,
            decimal? cost,
            CurrencyId? costCurrencyId,
            decimal? costExchangeRate,
            decimal? price,
            CurrencyId? priceCurrencyId,
            decimal? priceExchangeRate,
            bool isSalable,
            bool isPurchasable,
            bool isStored)
        {
            if (string.IsNullOrWhiteSpace(code))
                return ProductErrors.CodeIsRequired;

            if (string.IsNullOrWhiteSpace(name))
                return ProductErrors.NameIsRequired;

            if (productServiceLineId.Value == Guid.Empty)
                return ProductErrors.InvalidProductServiceLine;

            if (cost.HasValue && cost < 0)
                return ProductErrors.NegativeCost;

            if (price.HasValue && price < 0)
                return ProductErrors.NegativePrice;

            var product = new Product(
                ProductId.New(),
                code.Trim(),
                itemNumberByDay,
                name.Trim(),
                productServiceLineId,
                cost,
                costCurrencyId,
                costExchangeRate,
                price,
                priceCurrencyId,
                priceExchangeRate,
                isSalable,
                isPurchasable,
                isStored);

            product.Raise(new ProductCreatedDomainEvent(
                product.Id,
                product.ProductServiceLineId,
                product.Code,
                product.Name));

            return product;
        }

        public ErrorOr<Success> UpdateDetails(
            string name,
            string? description,
            string? model,
            string? barcode,
            string? supplierCode,
            ProductTypeId? productTypeId,
            ProductCategoryId? productCategoryId,
            ProductSubCategoryId? productSubCategoryId,
            ProductBrandId? productBrandId,
            bool isSalable,
            bool isPurchasable,
            bool isStored,
            UnitId? purchaseUnitId,
            UnitId? saleUnitId,
            decimal? unitConversionFactor,
            decimal? weight,
            decimal? volume,
            decimal? contentCapacity,
            string? tags,
            string? imageUrl,
            decimal? profitMargin)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductErrors.NameIsRequired;

            Name = name.Trim();
            Description = description?.Trim();
            Model = model?.Trim();
            Barcode = barcode?.Trim();
            ClientCode = supplierCode?.Trim();
            ProductTypeId = productTypeId;
            ProductCategoryId = productCategoryId;
            ProductSubCategoryId = productSubCategoryId;
            ProductBrandId = productBrandId;
            IsSalable = isSalable;
            IsPurchasable = isPurchasable;
            IsStored = isStored;
            PurchaseUnitId = purchaseUnitId;
            SaleUnitId = saleUnitId;
            UnitConversionFactor = unitConversionFactor;
            Weight = weight;
            Volume = volume;
            ContentCapacity = contentCapacity;
            Tags = tags?.Trim();
            ImageUrl = imageUrl?.Trim();
            ProfitMargin = profitMargin;

            return Result.Success;
        }

        public ErrorOr<Success> UpdatePrices(
            Guid changedByUserId,
            decimal? newCost,
            decimal? newCostBase,
            CurrencyId? newCostCurrencyId,
            decimal? newCostExchangeRate,
            decimal? newPrice,
            decimal? newPriceBase,
            CurrencyId? newPriceCurrencyId,
            decimal? newPriceExchangeRate,
            decimal? newPrice2,
            decimal? newPrice3,
            decimal? newPrice4,
            decimal? newPrice5,
            IEnumerable<TaxId> newPurchaseTaxIds,
            IEnumerable<TaxId> newSaleTaxIds,
            decimal? oldPurchaseTaxRate,
            decimal? newPurchaseTaxRate,
            decimal? oldSaleTaxRate,
            decimal? newSaleTaxRate,
            decimal? newProfitMargin)
        {
            if (newCost.HasValue && newCost < 0)
                return ProductErrors.NegativeCost;

            if (newPrice.HasValue && newPrice < 0)
                return ProductErrors.NegativePrice;

            var history = new ProductPriceHistory(
                Id,
                changedByUserId,
                Cost, CostBase, CostCurrencyId, CostExchangeRate,
                newCost, newCostBase, newCostCurrencyId, newCostExchangeRate,
                Price, PriceBase, PriceCurrencyId, PriceExchangeRate,
                newPrice, newPriceBase, newPriceCurrencyId, newPriceExchangeRate,
                Price2, newPrice2,
                Price3, newPrice3,
                Price4, newPrice4,
                Price5, newPrice5,
                oldPurchaseTaxRate, newPurchaseTaxRate,
                oldSaleTaxRate, newSaleTaxRate,
                ProfitMargin, newProfitMargin);

            _priceHistory.Add(history);

            Cost = newCost;
            CostBase = newCostBase;
            CostCurrencyId = newCostCurrencyId;
            CostExchangeRate = newCostExchangeRate;
            Price = newPrice;
            PriceBase = newPriceBase;
            PriceCurrencyId = newPriceCurrencyId;
            PriceExchangeRate = newPriceExchangeRate;
            Price2 = newPrice2;
            Price3 = newPrice3;
            Price4 = newPrice4;
            Price5 = newPrice5;
            ProfitMargin = newProfitMargin;

            SetTaxes(newPurchaseTaxIds, newSaleTaxIds);

            return Result.Success;
        }

        public void UpdateCode(string newCode)
        {
            if (!string.IsNullOrWhiteSpace(newCode))
                Code = newCode.Trim();
        }

        public void SetTaxes(IEnumerable<TaxId> purchaseTaxIds, IEnumerable<TaxId> saleTaxIds)
        {
            _purchaseTaxIds.Clear();
            _purchaseTaxIds.AddRange(purchaseTaxIds);
            _saleTaxIds.Clear();
            _saleTaxIds.AddRange(saleTaxIds);
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
