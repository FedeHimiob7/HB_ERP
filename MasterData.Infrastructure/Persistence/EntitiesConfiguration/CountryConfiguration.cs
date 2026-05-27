using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence.EntitiesConfiguration
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => CountryId.Create(value));

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Name).IsUnique();
            builder.HasQueryFilter(c => c.IsActive);
        }
    }
}
