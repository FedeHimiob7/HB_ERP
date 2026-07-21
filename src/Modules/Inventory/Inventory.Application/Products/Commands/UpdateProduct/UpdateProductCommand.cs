using ErrorOr;
using Inventory.Application.Products.Models;
using MediatR;

namespace Inventory.Application.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string? Code,
        string Name,
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
        Guid? PurchaseUnitId,
        Guid? SaleUnitId,
        decimal? UnitConversionFactor,
        decimal? Weight,
        decimal? Volume,
        decimal? ContentCapacity,
        List<Guid> PurchaseTaxIds,
        List<Guid> SaleTaxIds,
        string? Tags,
        string? ImageUrl,
        decimal? ProfitMargin) : IRequest<ErrorOr<ProductResponse>>;
}
