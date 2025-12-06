using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _db;

        public RoleRepository(IdentityDbContext dbContext)
        {
            _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Role?> GetByIdAsync(RoleId id)
        {
            var entity = await _db.Roles
                .FirstOrDefaultAsync(r => r.Id == id.Value);

            if (entity is null)
                return null;

            return RoleMapper.ToDomain(entity);
        }
        public async Task AddAsync(Role role)
        {
            var entity = RoleMapper.ToEntity(role);
            await _db.Roles.AddAsync(entity);
        }

        
    }
}
