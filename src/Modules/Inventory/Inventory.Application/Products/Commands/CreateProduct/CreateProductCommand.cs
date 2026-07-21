using ErrorOr;
using MediatR;

namespace Inventory.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Code,
        string Name,
        Guid ProductServiceLineId,
        string? Description,
        string? Model,
        string? Barcode,
        string? ClientCode,
        Guid? ProductTypeId,
        Guid? ProductCategoryId,
        Guid? ProductSubCategoryId,
        Guid? ProductBrandId,
        bool IsSalable,
        bool IsPurchasable,
        bool IsStored,
        decimal? Cost,
        Guid? CostCurrencyId,
        decimal? CostExchangeRate,
        decimal? Price,
        Guid? PriceCurrencyId,
        decimal? PriceExchangeRate,
        decimal? Price2,
        decimal? Price3,
        decimal? Price4,
        decimal? Price5,
        List<Guid> PurchaseTaxIds,
        List<Guid> SaleTaxIds,
        Guid? PurchaseUnitId,
        Guid? SaleUnitId,
        decimal? UnitConversionFactor,
        decimal? Weight,
        decimal? Volume,
        decimal? ContentCapacity,
        string? Tags,
        string? ImageUrl,
        decimal? ProfitMargin) : IRequest<ErrorOr<Guid>>;
}
