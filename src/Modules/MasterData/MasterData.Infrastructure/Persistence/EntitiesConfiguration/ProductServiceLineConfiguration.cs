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
    public class ProductServiceLineConfiguration : IEntityTypeConfiguration<ProductServiceLine>
    {
        public void Configure(EntityTypeBuilder<ProductServiceLine> builder)
        {
            builder.ToTable("ProductServiceLines");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => ProductServiceLineId.Create(value));

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(250); 

            // Filtro global para excluir los inactivos automáticamente en las consultas
            builder.HasQueryFilter(p => p.IsActive);
        }
    }
}
