using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using HB_ERP.SharedKernel.Infrastructure;
using HB_ERP.SharedKernel.Infrastructure.Extensions;
using MasterData.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Domain.Entities.Unit> Units { get; set; }


    }
}
