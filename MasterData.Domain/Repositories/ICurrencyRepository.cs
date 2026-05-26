using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetByIdAsync(CurrencyId id, CancellationToken cancellationToken = default);
        Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Currency> Currencies, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task AddAsync(Currency currency, CancellationToken cancellationToken = default);
        Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
