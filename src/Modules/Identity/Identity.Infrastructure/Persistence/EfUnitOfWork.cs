using Identity.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence
{
    internal sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _dbContext;

        public EfUnitOfWork(IdentityDbContext dbContext) => _dbContext = dbContext;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
