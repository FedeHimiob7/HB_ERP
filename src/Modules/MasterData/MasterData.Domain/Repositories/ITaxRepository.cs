using MasterData.Domain.Entities;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface ITaxRepository
    {
        Task<Tax?> GetByIdAsync(TaxId id, CancellationToken cancellationToken = default);
        Task<List<Tax>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Tax> Taxes, int TotalCount)> GetPagedAsync(TaxFilter filter, CancellationToken cancellationToken = default);
        Task AddAsync(Tax tax, CancellationToken cancellationToken = default);
        Task UpdateAsync(Tax tax, CancellationToken cancellationToken = default);
    }
}
