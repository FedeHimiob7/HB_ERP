using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => CityId.Create(value));

            builder.Property(c => c.StateId)
                .IsRequired()
                .HasConversion(id => id.Value, value => StateId.Create(value));

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasQueryFilter(c => c.IsActive);
        }
    }
}
