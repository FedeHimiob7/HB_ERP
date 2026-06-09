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
    public interface IStateRepository
    {
        Task<State?> GetByIdAsync(StateId id, CancellationToken cancellationToken = default);
        Task<State?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<List<State>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<State> States, int TotalCount)> GetPagedAsync(
            StateFilter filter,
            CancellationToken cancellationToken = default);

        Task AddAsync(State state, CancellationToken cancellationToken = default);
        Task UpdateAsync(State state, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
