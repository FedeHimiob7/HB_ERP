using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class FiscalTerminalConfiguration : IEntityTypeConfiguration<FiscalTerminal>
    {
        public void Configure(EntityTypeBuilder<FiscalTerminal> builder)
        {
            builder.ToTable("FiscalTerminals");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                .HasConversion(id => id.Value, value => FiscalTerminalId.Create(value));

            builder.Property(f => f.BranchId)
                .IsRequired()
                .HasConversion(id => id.Value, value => BranchId.Create(value));

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.EmissionMethod)
                .IsRequired();

            builder.HasQueryFilter(f => f.IsActive);
        }
    }
}
