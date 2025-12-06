using Identity.Domain.Entities;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(RoleId id);
        Task AddAsync(Role role);
    }
}
