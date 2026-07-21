using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Infrastructure;
using HB_ERP.SharedKernel.Infrastructure.Extensions;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence
{
    public sealed class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Inventory");
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);

            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.ToTable("OutboxMessages");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.ApplyAuditableShadowProperties();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductSubCategory> ProductSubCategories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<StorageType> StorageTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCodeCounter> ProductCodeCounters { get; set; }
    }
}
