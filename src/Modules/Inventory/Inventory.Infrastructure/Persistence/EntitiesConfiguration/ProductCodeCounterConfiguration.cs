using Inventory.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntitiesConfiguration
{
    public class ProductCodeCounterConfiguration : IEntityTypeConfiguration<ProductCodeCounter>
    {
        public void Configure(EntityTypeBuilder<ProductCodeCounter> builder)
        {
            builder.ToTable("ProductCodeCounters");

            builder.HasKey(x => new { x.PslId, x.Date, x.ItemNumberByDay });

            builder.Property(x => x.PslId)
                .IsRequired()
                .HasConversion(id => id.Value, value => ProductServiceLineId.Create(value));

            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.PslSequenceNumber).IsRequired();
            builder.Property(x => x.ItemNumberByDay).IsRequired();

            builder.Property(x => x.GeneratedCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IsConsumed).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => new { x.GeneratedCode, x.PslId })
                .IsUnique()
                .HasDatabaseName("IX_ProductCodeCounters_GeneratedCode_PslId");
        }
    }
}
