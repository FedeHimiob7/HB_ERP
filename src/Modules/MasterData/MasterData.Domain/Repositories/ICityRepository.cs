using MasterData.Domain.Entities;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface ICityRepository
    {
        Task<City?> GetByIdAsync(CityId id, CancellationToken cancellationToken = default);
        Task<List<City>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<City> Cities, int TotalCount)> GetPagedAsync(
            CityFilter filter,
            CancellationToken cancellationToken = default);

        Task AddAsync(City city, CancellationToken cancellationToken = default);
        Task UpdateAsync(City city, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameInStateAsync(string name, StateId stateId, CityId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
