using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Repositories
{
    public interface ICountryRepository
    {
        Task<Country?> GetByIdAsync(CountryId id, CancellationToken cancellationToken = default);
        Task<List<Country>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Country> Countries, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            CancellationToken cancellationToken = default);
        Task AddAsync(Country country, CancellationToken cancellationToken = default);
        Task UpdateAsync(Country country, CancellationToken cancellationToken = default);
    }
}
