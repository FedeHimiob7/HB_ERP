using Identity.Domain.Entities;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Repositories
{
    public interface ISystemActionRepository
    {
        Task<SystemAction?> GetByIdAsync(ActionsId id);
        Task UpdateAsync(SystemAction action);
        Task<List<SystemAction>> GetAllAsync();
        Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
        Task AddAsync(SystemAction systemAction, CancellationToken cancellationToken = default);
    }
}
