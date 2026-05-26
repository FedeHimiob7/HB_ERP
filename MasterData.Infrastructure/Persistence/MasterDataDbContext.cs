using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using HB_ERP.SharedKernel.Infrastructure;
using HB_ERP.SharedKernel.Infrastructure.Extensions;
using MassTransit;
using MasterData.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence
{
    public sealed class MasterDataDbContext : DbContext
    {
        public MasterDataDbContext(DbContextOptions<MasterDataDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MasterData");
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDataDbContext).Assembly);

            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.ToTable("OutboxMessages");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.ApplyAuditableShadowProperties();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ProductServiceLine> ProductServiceLines { get; set; }


    }
}
