using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductCodeCounterRepository : IProductCodeCounterRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductCodeCounterRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(int PslSequenceNumber, int ItemNumberByDay, string GeneratedCode)> ReserveNextAsync(
            ProductServiceLineId pslId,
            DateOnly date,
            Func<int, int, string> codeFactory,
            CancellationToken cancellationToken = default)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, cancellationToken);

            try
            {
                // UPDLOCK + HOLDLOCK garantiza que dos requests simultáneos no lean el mismo valor
                var existingForPsl = await _dbContext.ProductCodeCounters
                    .FromSqlRaw(
                        "SELECT * FROM [Inventory].[ProductCodeCounters] WITH (UPDLOCK, HOLDLOCK, ROWLOCK) WHERE PslId = {0}",
                        pslId.Value)
                    .ToListAsync(cancellationToken);

                int pslSequenceNumber;
                int nextItemNumber;

                if (existingForPsl.Count == 0)
                {
                    // PSL nuevo — asignar el siguiente número de secuencia global
                    var maxPslSeq = await _dbContext.ProductCodeCounters
                        .MaxAsync(x => (int?)x.PslSequenceNumber, cancellationToken) ?? 0;

                    pslSequenceNumber = maxPslSeq + 1;
                    nextItemNumber = 1;
                }
                else
                {
                    pslSequenceNumber = existingForPsl[0].PslSequenceNumber;

                    var maxForDay = existingForPsl
                        .Where(r => r.Date == date)
                        .Select(r => (int?)r.ItemNumberByDay)
                        .Max() ?? 0;

                    nextItemNumber = maxForDay + 1;
                }

                var generatedCode = codeFactory(pslSequenceNumber, nextItemNumber);

                var entry = ProductCodeCounter.Create(pslId, pslSequenceNumber, date, nextItemNumber, generatedCode);
                _dbContext.ProductCodeCounters.Add(entry);

                await _dbContext.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return (pslSequenceNumber, nextItemNumber, generatedCode);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<int?> ConsumeAsync(
            string generatedCode,
            ProductServiceLineId pslId,
            CancellationToken cancellationToken = default)
        {
            var reservation = await _dbContext.ProductCodeCounters
                .FirstOrDefaultAsync(
                    r => r.GeneratedCode == generatedCode && r.PslId == pslId && !r.IsConsumed,
                    cancellationToken);

            if (reservation is null) return null;

            reservation.Consume();

            return reservation.ItemNumberByDay;
        }
    }
}
