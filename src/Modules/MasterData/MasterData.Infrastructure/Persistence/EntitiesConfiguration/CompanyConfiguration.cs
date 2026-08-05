using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => CompanyId.Create(value));

            builder.Property(c => c.Rif)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.LegalName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.RegisteredAddress)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.TaxpayerType)
                .IsRequired();
        }
    }
}
