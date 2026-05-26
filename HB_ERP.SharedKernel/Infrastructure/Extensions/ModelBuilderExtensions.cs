using HB_ERP.SharedKernel.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyAuditableShadowProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType != null && typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).Property<DateTime>("CreatedAt").IsRequired();
                    modelBuilder.Entity(entityType.ClrType).Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
                    modelBuilder.Entity(entityType.ClrType).Property<DateTime?>("ModifiedAt");
                    modelBuilder.Entity(entityType.ClrType).Property<string>("ModifiedBy").HasMaxLength(100).IsRequired(false);
                }
            }
        }
    }
}
