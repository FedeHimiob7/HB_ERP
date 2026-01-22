using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    internal sealed class RoleNameUniquenessChecker : IRoleNameUniquenessChecker
    {
        private readonly IdentityDbContext _dbContext;

        public RoleNameUniquenessChecker(IdentityDbContext dbContext) => _dbContext = dbContext;


        public Task<bool> IsRoleNameUniqueAsync(string name, CancellationToken cancellationToken = default)
        {
            return _dbContext.Roles
                .AllAsync(x => x.Name != name.ToUpperInvariant(), cancellationToken);
        }
    }
    
}
