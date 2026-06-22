using Inventory.Domain.Entities;
using Inventory.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntitiesConfiguration
{
    public class ProductBrandConfiguration : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(EntityTypeBuilder<ProductBrand> builder)
        {
            builder.ToTable("ProductBrands");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => ProductBrandId.Create(value));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasQueryFilter(x => x.IsActive);
        }
    }
}
