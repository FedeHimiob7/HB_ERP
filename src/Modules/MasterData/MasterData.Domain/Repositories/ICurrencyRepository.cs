using MasterData.Domain.Entities;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetByIdAsync(CurrencyId id, CancellationToken cancellationToken = default);
        Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Currency> Currencies, int TotalCount)> GetPagedAsync(
            CurrencyFilter filter,
            CancellationToken cancellationToken = default);
        Task AddAsync(Currency currency, CancellationToken cancellationToken = default);
        Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
