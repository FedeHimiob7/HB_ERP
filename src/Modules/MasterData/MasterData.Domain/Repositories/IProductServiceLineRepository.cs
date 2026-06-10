using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Repositories
{
    public interface IProductServiceLineRepository
    {
        Task<ProductServiceLine?> GetByIdAsync(ProductServiceLineId id, CancellationToken cancellationToken = default);
        Task<List<ProductServiceLine>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<ProductServiceLine> ProductServiceLines, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            CancellationToken cancellationToken = default);
        Task AddAsync(ProductServiceLine productServiceLine, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductServiceLine productServiceLine, CancellationToken cancellationToken = default);
    }
}
