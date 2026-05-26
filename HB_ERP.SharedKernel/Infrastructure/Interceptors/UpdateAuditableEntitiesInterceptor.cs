using HB_ERP.SharedKernel.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Infrastructure.Interceptors
{
    public sealed class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public UpdateAuditableEntitiesInterceptor(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                var auditableEntries = eventData.Context.ChangeTracker.Entries<IAuditable>();

                var currentUserId = _currentUserProvider.UserId ?? "System";

                foreach (var entry in auditableEntries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                        entry.Property("CreatedBy").CurrentValue = currentUserId;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        entry.Property("ModifiedAt").CurrentValue = DateTime.UtcNow;
                        entry.Property("ModifiedBy").CurrentValue = currentUserId;
                    }
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
