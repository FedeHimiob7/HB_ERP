using HB_ERP.SharedKernel.Infrastructure;
using MassTransit;
using MasterData.Infrastructure.Persistence.Entities.CurrencyEntity;
using Microsoft.EntityFrameworkCore;
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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDataDbContext).Assembly);

            modelBuilder.HasDefaultSchema("MasterData");

            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.ToTable("OutboxMessages"); 
                builder.HasKey(x => x.Id);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
