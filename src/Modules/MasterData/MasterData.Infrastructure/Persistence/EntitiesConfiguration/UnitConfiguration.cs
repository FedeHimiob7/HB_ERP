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
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Units");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, value => UnitId.Create(value));

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50); 

            builder.Property(u => u.Description)
                .IsRequired()
                .HasMaxLength(150); 

            
            builder.HasIndex(u => u.Name).IsUnique();
            builder.HasQueryFilter(u => u.IsActive);
        }
    }
}
