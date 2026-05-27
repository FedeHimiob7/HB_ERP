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
    public class StateConfiguration : IEntityTypeConfiguration<State>
    {
        public void Configure(EntityTypeBuilder<State> builder)
        {
            builder.ToTable("States");

            // Primary Key
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .HasConversion(id => id.Value, value => StateId.Create(value));

            // Foreign Key a Country (Strongly-Typed)
            builder.Property(s => s.CountryId)
                .IsRequired()
                .HasConversion(id => id.Value, value => CountryId.Create(value));

            builder.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.Code).IsUnique();
            builder.HasQueryFilter(s => s.IsActive);
        }
    }
}
