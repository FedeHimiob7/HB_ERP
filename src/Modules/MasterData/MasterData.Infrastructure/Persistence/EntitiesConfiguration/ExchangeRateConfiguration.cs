using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.ToTable("ExchangeRates");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasConversion(id => id.Value, value => ExchangeRateId.Create(value));

            builder.Property(e => e.Rate)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(e => e.RegisterDate)
                .IsRequired();

            builder.Property(e => e.Source)
                .IsRequired()
                .HasConversion<int>();

            builder.HasIndex(e => e.RegisterDate);
        }
    }
}
