using Identity.Domain.Entities;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(RoleId id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
        Task<List<Role>> GetAllAsync();
        Task AddAsync(Role role);
    }
}
