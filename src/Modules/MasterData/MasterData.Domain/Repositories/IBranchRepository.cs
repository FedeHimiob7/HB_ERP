using ErrorOr;
using MasterData.Domain.Entities;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface IBranchRepository
    {
        Task<Branch?> GetByIdAsync(BranchId id, CancellationToken cancellationToken = default);
        Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Branch> Branches, int TotalCount)> GetPagedAsync(
            BranchFilter filter,
            CancellationToken cancellationToken = default);

        Task<ErrorOr<Branch>> ReserveNextSequenceNumberAndAddAsync(
            Func<int, ErrorOr<Branch>> factory,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(Branch branch, CancellationToken cancellationToken = default);
    }
}
