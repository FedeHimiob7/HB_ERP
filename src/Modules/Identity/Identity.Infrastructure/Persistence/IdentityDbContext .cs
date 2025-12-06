using Identity.Domain;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence
{
    public sealed class IdentityDbContext : DbContext
    {
        private readonly IPublisher _publisher;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IPublisher publisher)
            : base(options)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<RoleEntity> Roles => Set<RoleEntity>();
        public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();



        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var aggregates = ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity != null &&
                    e.Entity.GetType().BaseType is { IsGenericType: true } baseType &&
                    baseType.GetGenericTypeDefinition() == typeof(AggregateRoot<>))
                .Select(e => (AggregateRoot<object>)e.Entity)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
                await _publisher.Publish(domainEvent, cancellationToken);

            foreach (var aggregate in aggregates)
                aggregate.ClearDomainEvents();

            return result;
        }
    }
}
