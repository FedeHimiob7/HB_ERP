using Identity.Domain;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    internal sealed class UserEmailUniquenessChecker : IUserEmailUniquenessChecker
    {
        private readonly IdentityDbContext _dbContext;

        public UserEmailUniquenessChecker(IdentityDbContext dbContext) => _dbContext = dbContext;

        public Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _dbContext.Users
                .AllAsync(x => x.NormalizedEmail != email.Value.ToUpperInvariant(), cancellationToken);
        }
    }
}
