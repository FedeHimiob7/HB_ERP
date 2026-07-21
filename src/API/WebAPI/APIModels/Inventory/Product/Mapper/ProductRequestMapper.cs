using Inventory.Application.Products.Commands.CreateProduct;
using Inventory.Application.Products.Commands.UpdateProduct;
using Inventory.Application.Products.Commands.UpdateProductPrices;
using Inventory.Application.Products.Queries.CalculatePrices;

namespace WebAPI.APIModels.Inventory.Product.Mapper
{
    internal static class ProductRequestMapper
    {
        public static CalculatePricesQuery ToCalculatePricesQuery(this CalculatePricesRequest r) =>
            new(r.BaseAmount,
                r.TaxList.Select(t => new TaxItemQuery(t.TaxId, t.Rate, t.IsIGTF)).ToList(),
                r.Profit,
                r.Commission,
                r.IsCost,
                r.Cost);

        public static CreateProductCommand ToCreateCommand(this CreateProductRequest r) =>
            new(r.Code,
                r.Name,
                r.ProductServiceLineId,
                r.Description,
                r.Model,
                r.Barcode,
                r.ClientCode,
                r.ProductTypeId,
                r.ProductCategoryId,
                r.ProductSubCategoryId,
                r.ProductBrandId,
                r.IsSalable,
                r.IsPurchasable,
                r.IsStored,
                r.Cost,
                r.CostCurrencyId,
                r.CostExchangeRate,
                r.Price,
                r.PriceCurrencyId,
                r.PriceExchangeRate,
                r.Price2,
                r.Price3,
                r.Price4,
                r.Price5,
                r.PurchaseTaxIds,
                r.SaleTaxIds,
                r.PurchaseUnitId,
                r.SaleUnitId,
                r.UnitConversionFactor,
                r.Weight,
                r.Volume,
                r.ContentCapacity,
                r.Tags,
                r.ImageUrl,
                r.ProfitMargin);

        public static UpdateProductCommand ToUpdateCommand(this UpdateProductRequest r, Guid id) =>
            new(id,
                r.Code,
                r.Name,
                r.Description,
                r.Model,
                r.Barcode,
                r.ClientCode,
                r.ProductTypeId,
                r.ProductCategoryId,
                r.ProductSubCategoryId,
                r.ProductBrandId,
                r.IsSalable,
                r.IsPurchasable,
                r.IsStored,
                r.PurchaseUnitId,
                r.SaleUnitId,
                r.UnitConversionFactor,
                r.Weight,
                r.Volume,
                r.ContentCapacity,
                r.PurchaseTaxIds,
                r.SaleTaxIds,
                r.Tags,
                r.ImageUrl,
                r.ProfitMargin);

        public static UpdateProductPricesCommand ToUpdatePricesCommand(this UpdateProductPricesRequest r, Guid id) =>
            new(id,
                r.Cost,
                r.CostCurrencyId,
                r.CostExchangeRate,
                r.Price,
                r.PriceCurrencyId,
                r.PriceExchangeRate,
                r.Price2,
                r.Price3,
                r.Price4,
                r.Price5);
    }
}
