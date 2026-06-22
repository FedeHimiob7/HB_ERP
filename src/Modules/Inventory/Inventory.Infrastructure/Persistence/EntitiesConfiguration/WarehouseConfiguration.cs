using Inventory.Domain.Entities;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntitiesConfiguration
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => WarehouseId.Create(value));

            builder.Property(x => x.ProductServiceLineId)
                .IsRequired()
                .HasConversion(id => id.Value, value => ProductServiceLineId.Create(value));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(250);

            builder.Property(x => x.Latitude)
                .HasMaxLength(50);

            builder.Property(x => x.Longitude)
                .HasMaxLength(50);

            builder.HasIndex(x => new { x.Name, x.ProductServiceLineId })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasQueryFilter(x => x.IsActive);
        }
    }
}
