using MasterData.Domain.Entities;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface IFiscalTerminalRepository
    {
        Task<FiscalTerminal?> GetByIdAsync(FiscalTerminalId id, CancellationToken cancellationToken = default);
        Task<List<FiscalTerminal>> GetAllAsync(BranchId? branchId = null, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<FiscalTerminal> FiscalTerminals, int TotalCount)> GetPagedAsync(
            FiscalTerminalFilter filter,
            CancellationToken cancellationToken = default);

        Task AddAsync(FiscalTerminal fiscalTerminal, CancellationToken cancellationToken = default);
        Task UpdateAsync(FiscalTerminal fiscalTerminal, CancellationToken cancellationToken = default);
    }
}
