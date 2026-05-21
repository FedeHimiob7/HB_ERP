using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Entities
{
    internal sealed class RoleActionEntityConfiguration : IEntityTypeConfiguration<RoleActionEntity>
    {
        public void Configure(EntityTypeBuilder<RoleActionEntity> builder)
        {
            builder.ToTable("RoleActions");

            // Llave primaria compuesta
            builder.HasKey(ra => new { ra.RoleId, ra.ActionId });

            // Relación con Role
            builder.HasOne(ra => ra.Role)
                   .WithMany(r => r.RoleActions)
                   .HasForeignKey(ra => ra.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación con SystemAction
            builder.HasOne(ra => ra.SystemAction)
                   .WithMany() 
                   .HasForeignKey(ra => ra.ActionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
