namespace WebAPI.APIModels.Inventory.Product
{
    public record UpdateProductRequest(
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
        decimal? ProfitMargin);
}
