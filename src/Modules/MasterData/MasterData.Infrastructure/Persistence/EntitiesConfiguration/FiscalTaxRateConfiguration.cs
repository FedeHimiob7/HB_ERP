using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class FiscalTaxRateConfiguration : IEntityTypeConfiguration<FiscalTaxRate>
    {
        public void Configure(EntityTypeBuilder<FiscalTaxRate> builder)
        {
            builder.ToTable("FiscalTaxRates");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasConversion(id => id.Value, value => FiscalTaxRateId.Create(value));

            builder.Property(r => r.TaxId)
                .HasConversion(id => id.Value, value => TaxId.Create(value))
                .IsRequired();

            builder.Property(r => r.Rate)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(r => r.EffectiveFrom)
                .IsRequired();

            builder.HasIndex(r => new { r.TaxId, r.EffectiveFrom });
        }
    }
}
