using HB_ERP.SharedKernel.Infrastructure;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MasterData.Infrastructure.Persistence.Entities.CurrencyEntity;
using MasterData.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class CurrencyRepository : ICurrencyRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public CurrencyRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Currency?> GetByIdAsync(CurrencyId id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Set<CurrencyEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id.Value && e.IsActive, cancellationToken);

            return entity is null ? null : CurrencyMapper.ToDomain(entity);
        }

        public async Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Set<CurrencyEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == code && e.IsActive, cancellationToken);

            return entity is null ? null : CurrencyMapper.ToDomain(entity);
        }      

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<CurrencyEntity>()
                .AsNoTracking()
                .AnyAsync(e => e.Code == code, cancellationToken);
        }

        public async Task AddAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            var entity = CurrencyMapper.ToEntity(currency);
            await _dbContext.Set<CurrencyEntity>().AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            var entity = CurrencyMapper.ToEntity(currency);
            _dbContext.Set<CurrencyEntity>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _dbContext.Set<CurrencyEntity>()
                .AsNoTracking()
                .Where(e => e.IsActive) 
                .ToListAsync(cancellationToken);

            return entities.Select(CurrencyMapper.ToDomain).ToList();
        }

        public async Task<(IReadOnlyList<Currency> Currencies, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<CurrencyEntity>()
                .AsNoTracking()
                .Where(e => e.IsActive);

            var totalCount = await query.CountAsync(cancellationToken);

            var entities = await query
                .OrderBy(c => c.Code)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var currencies = entities.Select(CurrencyMapper.ToDomain).ToList();

            return (currencies, totalCount);
        }
    }
}
