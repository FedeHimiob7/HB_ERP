using MasterData.Domain.Entities;

namespace MasterData.Domain.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetSingletonAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Company company, CancellationToken cancellationToken = default);
        Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
    }
}
