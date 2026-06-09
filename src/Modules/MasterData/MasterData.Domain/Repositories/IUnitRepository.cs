using MasterData.Domain.Entities;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Repositories
{
    public interface IUnitRepository
    {
        Task<Unit?> GetByIdAsync(UnitId id, CancellationToken cancellationToken = default);
        Task<List<Unit>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Unit> Units, int TotalCount)> GetPagedAsync(UnitFilter filter, CancellationToken cancellationToken = default);
        Task AddAsync(Unit unit, CancellationToken cancellationToken = default);
        Task UpdateAsync(Unit unit, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
