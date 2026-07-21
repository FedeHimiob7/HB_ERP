using Inventory.Domain.Entities;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntitiesConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => ProductId.Create(value));

            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ItemNumberByDay).IsRequired();
            builder.Property(x => x.Barcode).HasMaxLength(50);
            builder.Property(x => x.ClientCode).HasMaxLength(50);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(70);
            builder.Property(x => x.Description).HasMaxLength(700);
            builder.Property(x => x.Model).HasMaxLength(75);

            builder.Property(x => x.ProductServiceLineId)
                .IsRequired()
                .HasConversion(id => id.Value, value => ProductServiceLineId.Create(value));

            builder.Property(x => x.ProductTypeId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new ProductTypeId(value.Value) : (ProductTypeId?)null);

            builder.Property(x => x.ProductCategoryId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new ProductCategoryId(value.Value) : (ProductCategoryId?)null);

            builder.Property(x => x.ProductSubCategoryId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new ProductSubCategoryId(value.Value) : (ProductSubCategoryId?)null);

            builder.Property(x => x.ProductBrandId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new ProductBrandId(value.Value) : (ProductBrandId?)null);

            builder.Property(x => x.IsSalable).IsRequired();
            builder.Property(x => x.IsPurchasable).IsRequired();
            builder.Property(x => x.IsStored).IsRequired();

            builder.Property(x => x.Cost).HasColumnType("decimal(18,4)");
            builder.Property(x => x.CostCurrencyId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? CurrencyId.Create(value.Value) : (CurrencyId?)null);
            builder.Property(x => x.CostExchangeRate).HasColumnType("decimal(18,6)");

            builder.Property(x => x.Price).HasColumnType("decimal(18,4)");
            builder.Property(x => x.PriceCurrencyId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? CurrencyId.Create(value.Value) : (CurrencyId?)null);
            builder.Property(x => x.PriceExchangeRate).HasColumnType("decimal(18,6)");

            builder.Property(x => x.Price2).HasColumnType("decimal(18,4)");
            builder.Property(x => x.Price3).HasColumnType("decimal(18,4)");
            builder.Property(x => x.Price4).HasColumnType("decimal(18,4)");
            builder.Property(x => x.Price5).HasColumnType("decimal(18,4)");

            builder.Property(x => x.PurchaseUnitId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new UnitId(value.Value) : (UnitId?)null);

            builder.Property(x => x.SaleUnitId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new UnitId(value.Value) : (UnitId?)null);

            builder.Property(x => x.UnitConversionFactor).HasColumnType("decimal(18,6)");
            builder.Property(x => x.Weight).HasColumnType("decimal(18,4)");
            builder.Property(x => x.Volume).HasColumnType("decimal(18,4)");
            builder.Property(x => x.ContentCapacity).HasColumnType("decimal(18,4)");
            builder.Property(x => x.Tags).HasMaxLength(500);
            builder.Property(x => x.ImageUrl).HasMaxLength(500);
            builder.Property(x => x.ProfitMargin).HasColumnType("decimal(18,4)");

            // Tax IDs stored as JSON strings via shadow properties
            builder.Property<string>("PurchaseTaxIdsJson")
                .HasColumnName("PurchaseTaxIds")
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("[]");

            builder.Property<string>("SaleTaxIdsJson")
                .HasColumnName("SaleTaxIds")
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("[]");

            // Price history as owned collection
            builder.OwnsMany(p => p.PriceHistory, ph =>
            {
                ph.ToTable("ProductPriceHistories");
                ph.HasKey(x => x.Id);
                ph.Property(x => x.Id).ValueGeneratedNever();

                ph.WithOwner().HasForeignKey(nameof(ProductPriceHistory.ProductId));
                ph.Property(x => x.ProductId)
                    .HasConversion(id => id.Value, value => ProductId.Create(value))
                    .IsRequired();

                ph.Property(x => x.ChangedAt).IsRequired();
                ph.Property(x => x.ChangedByUserId).IsRequired();

                ph.Property(x => x.OldCost).HasColumnType("decimal(18,4)");
                ph.Property(x => x.OldCostCurrencyId)
                    .HasConversion(
                        id => id.HasValue ? id.Value.Value : (Guid?)null,
                        value => value.HasValue ? CurrencyId.Create(value.Value) : (CurrencyId?)null);
                ph.Property(x => x.OldCostExchangeRate).HasColumnType("decimal(18,6)");

                ph.Property(x => x.NewCost).HasColumnType("decimal(18,4)");
                ph.Property(x => x.NewCostCurrencyId)
                    .HasConversion(
                        id => id.HasValue ? id.Value.Value : (Guid?)null,
                        value => value.HasValue ? CurrencyId.Create(value.Value) : (CurrencyId?)null);
                ph.Property(x => x.NewCostExchangeRate).HasColumnType("decimal(18,6)");

                ph.Property(x => x.OldPrice).HasColumnType("decimal(18,4)");
                ph.Property(x => x.OldPriceCurrencyId)
                    .HasConversion(
                        id => id.HasValue ? id.Value.Value : (Guid?)null,
                        value => value.HasValue ? CurrencyId.Create(value.Value) : (CurrencyId?)null);
                ph.Property(x => x.OldPriceExchangeRate).HasColumnType("decimal(18,6)");

                ph.Property(x => x.NewPrice).HasColumnType("decimal(18,4)");
                ph.Property(x => x.NewPriceCurrencyId)
                    .HasConversion(
                        id => id.HasValue ? id.Value.Value : (Guid?)null,
                        value => value.HasValue ? CurrencyId.Create(value.Value) : (CurrencyId?)null);
                ph.Property(x => x.NewPriceExchangeRate).HasColumnType("decimal(18,6)");

                ph.Property(x => x.OldPrice2).HasColumnType("decimal(18,4)");
                ph.Property(x => x.NewPrice2).HasColumnType("decimal(18,4)");
                ph.Property(x => x.OldPrice3).HasColumnType("decimal(18,4)");
                ph.Property(x => x.NewPrice3).HasColumnType("decimal(18,4)");
                ph.Property(x => x.OldPrice4).HasColumnType("decimal(18,4)");
                ph.Property(x => x.NewPrice4).HasColumnType("decimal(18,4)");
                ph.Property(x => x.OldPrice5).HasColumnType("decimal(18,4)");
                ph.Property(x => x.NewPrice5).HasColumnType("decimal(18,4)");
            });

            builder.HasIndex(x => new { x.Code, x.ProductServiceLineId }).IsUnique();
            builder.HasQueryFilter(x => x.IsActive);
        }
    }
}
