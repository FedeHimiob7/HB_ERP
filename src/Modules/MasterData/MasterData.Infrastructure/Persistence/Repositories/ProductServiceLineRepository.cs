using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductServiceLineRepository : IProductServiceLineRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public ProductServiceLineRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductServiceLine?> GetByIdAsync(ProductServiceLineId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductServiceLines
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public async Task AddAsync(ProductServiceLine productServiceLine, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductServiceLines.AddAsync(productServiceLine, cancellationToken);
        }

        public Task UpdateAsync(ProductServiceLine productServiceLine, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductServiceLines.Update(productServiceLine);
            return Task.CompletedTask;
        }

        public async Task<List<ProductServiceLine>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductServiceLines
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<ProductServiceLine> ProductServiceLines, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductServiceLines.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var psls = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (psls, totalCount);
        }
    }
}
