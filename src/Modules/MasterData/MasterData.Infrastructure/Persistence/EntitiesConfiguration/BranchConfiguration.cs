using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => BranchId.Create(value));

            builder.Property(b => b.CompanyId)
                .IsRequired()
                .HasConversion(id => id.Value, value => CompanyId.Create(value));

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.SequenceNumber)
                .IsRequired();

            builder.HasIndex(b => b.SequenceNumber).IsUnique();

            builder.HasQueryFilter(b => b.IsActive);
        }
    }
}
