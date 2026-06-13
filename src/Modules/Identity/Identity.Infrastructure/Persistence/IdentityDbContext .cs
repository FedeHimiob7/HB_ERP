using HB_ERP.SharedKernel.Domain.Primitives;
using HB_ERP.SharedKernel.Infrastructure;
using Identity.Domain;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Entities;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence
{
    public sealed class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
            

            modelBuilder.HasDefaultSchema("Identity");
            modelBuilder.HasAnnotation("Relational:MigrationHistoryTable", "__EFMigrationsHistory");         

            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.ToTable("OutboxMessages");
                builder.HasKey(x => x.Id);
            });


            base.OnModelCreating(modelBuilder);

        }


        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<RoleEntity> Roles => Set<RoleEntity>();
        public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
        public DbSet<UserPslEntity> UserPsls => Set<UserPslEntity>();
    }
}
