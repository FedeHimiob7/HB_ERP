using Identity.Domain.Entities;
using Identity.Domain.VO;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Entities.SystemActionEntity
{
    internal sealed class SystemActionConfiguration : IEntityTypeConfiguration<SystemActionEntity>
    {
        public void Configure(EntityTypeBuilder<SystemActionEntity> builder)
        {
            builder.ToTable("SystemActions");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasIndex(e => e.Name)
                   .IsUnique();

            builder.Property(e => e.Description)
                   .HasMaxLength(255);

            builder.Property(e => e.IsActive)
                   .IsRequired();
        }
    }
}
