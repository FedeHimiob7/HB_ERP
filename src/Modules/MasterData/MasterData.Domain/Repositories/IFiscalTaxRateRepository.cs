using MasterData.Domain.Entities;
using MasterData.Domain.VO;

namespace MasterData.Domain.Repositories
{
    public interface IFiscalTaxRateRepository
    {
        Task<FiscalTaxRate?> GetEffectiveAsync(TaxId taxId, DateOnly asOfDate, CancellationToken cancellationToken = default);

        Task<Dictionary<TaxId, FiscalTaxRate>> GetEffectiveManyAsync(
            IEnumerable<TaxId> taxIds, DateOnly asOfDate, CancellationToken cancellationToken = default);

        Task AddAsync(FiscalTaxRate fiscalTaxRate, CancellationToken cancellationToken = default);
    }
}
