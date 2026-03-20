using Identity.Domain.Entities;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Repositories
{
    public interface ISystemActionRepositories
    {
        Task<SystemAction?> GetByIdAsync(ActionsId id);
        Task UpdateAsync(SystemAction action);
        Task<List<SystemAction>> GetAllAsync();
        Task AddAsync(SystemAction action);
    }
}
