using Inventory.Domain.Entities;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntitiesConfiguration
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => ProductCategoryId.Create(value));

            builder.Property(x => x.ProductServiceLineId)
                .IsRequired()
                .HasConversion(id => id.Value, value => ProductServiceLineId.Create(value));

            builder.Property(x => x.ProductTypeId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new ProductTypeId(value.Value) : (ProductTypeId?)null);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            builder.HasIndex(x => new { x.Name, x.ProductServiceLineId })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasQueryFilter(x => x.IsActive);
        }
    }
}
