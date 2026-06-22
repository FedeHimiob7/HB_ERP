using MasterData.Domain.Entities;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface IExchangeRateRepository
    {
        Task<ExchangeRate?> GetByIdAsync(ExchangeRateId id, CancellationToken cancellationToken = default);
        Task<ExchangeRate?> GetLatestAsync(CancellationToken cancellationToken = default);
        Task<ExchangeRate?> GetLatestByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<ExchangeRate> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(ExchangeRate exchangeRate, CancellationToken cancellationToken = default);
    }
}
