using Inventory.Domain.Entities;
using Inventory.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntitiesConfiguration
{
    public class ProductSubCategoryConfiguration : IEntityTypeConfiguration<ProductSubCategory>
    {
        public void Configure(EntityTypeBuilder<ProductSubCategory> builder)
        {
            builder.ToTable("ProductSubCategories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => ProductSubCategoryId.Create(value));

            builder.Property(x => x.ProductCategoryId)
                .IsRequired()
                .HasConversion(id => id.Value, value => ProductCategoryId.Create(value));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            builder.HasIndex(x => new { x.Name, x.ProductCategoryId })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasQueryFilter(x => x.IsActive);
        }
    }
}
