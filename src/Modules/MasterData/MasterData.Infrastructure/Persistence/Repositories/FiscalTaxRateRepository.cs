using HB_ERP.SharedKernel.Infrastructure.Extensions;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class FiscalTaxRateRepository : IFiscalTaxRateRepository
    {
        private readonly MasterDataDbContext _dbContext;
        public FiscalTaxRateRepository(MasterDataDbContext dbContext) => _dbContext = dbContext;

        public async Task<FiscalTaxRate?> GetEffectiveAsync(TaxId taxId, DateOnly asOfDate, CancellationToken cancellationToken = default)
            => await _dbContext.FiscalTaxRates.AsNoTracking()
                .Where(r => r.TaxId == taxId)
                .GetEffectiveAsOfAsync(asOfDate, cancellationToken);

        public async Task<Dictionary<TaxId, FiscalTaxRate>> GetEffectiveManyAsync(
            IEnumerable<TaxId> taxIds, DateOnly asOfDate, CancellationToken cancellationToken = default)
        {
            var ids = taxIds.Distinct().ToList();
            if (ids.Count == 0) return new Dictionary<TaxId, FiscalTaxRate>();

            var upperBound = asOfDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var candidates = await _dbContext.FiscalTaxRates.AsNoTracking()
                .Where(r => ids.Contains(r.TaxId) && r.EffectiveFrom < upperBound)
                .ToListAsync(cancellationToken);

            return candidates
                .GroupBy(r => r.TaxId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.EffectiveFrom).First());
        }

        public async Task AddAsync(FiscalTaxRate fiscalTaxRate, CancellationToken cancellationToken = default)
            => await _dbContext.FiscalTaxRates.AddAsync(fiscalTaxRate, cancellationToken);
    }
}
