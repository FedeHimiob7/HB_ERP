using Inventory.Domain.Entities;

namespace Inventory.Application.Products.Models
{
    internal static class ProductMapper
    {
        internal static ProductResponse ToResponse(Product p) => new(
            p.Id.Value,
            p.Code,
            p.ItemNumberByDay,
            p.Barcode,
            p.ClientCode,
            p.Name,
            p.Description,
            p.Model,
            p.ProductServiceLineId.Value,
            p.ProductTypeId?.Value,
            p.ProductCategoryId?.Value,
            p.ProductSubCategoryId?.Value,
            p.ProductBrandId?.Value,
            p.IsSalable,
            p.IsPurchasable,
            p.IsStored,
            p.Cost,
            p.CostCurrencyId?.Value,
            p.CostExchangeRate,
            p.Price,
            p.PriceCurrencyId?.Value,
            p.PriceExchangeRate,
            p.Price2,
            p.Price3,
            p.Price4,
            p.Price5,
            p.PurchaseTaxIds.Select(t => t.Value).ToList(),
            p.SaleTaxIds.Select(t => t.Value).ToList(),
            p.PurchaseUnitId?.Value,
            p.SaleUnitId?.Value,
            p.UnitConversionFactor,
            p.Weight,
            p.Volume,
            p.ContentCapacity,
            p.Tags,
            p.ImageUrl,
            p.ProfitMargin);
    }
}
