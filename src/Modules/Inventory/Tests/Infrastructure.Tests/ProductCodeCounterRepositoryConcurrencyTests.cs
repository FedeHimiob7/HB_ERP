using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests
{
    // Mismo patrón que BranchRepositoryConcurrencyTests (ver D-7/roadmap) — acá el lock es el
    // original de F0, el que ya usaba ProductCodeCounter y que luego se replicó en Branch para
    // el segmento de sucursal del código de producto.
    public sealed class ProductCodeCounterRepositoryConcurrencyTests : IAsyncLifetime
    {
        private readonly string _connectionString =
            $"Server=(localdb)\\MSSQLLocalDB;Database=HB_ERP_Test_Counter_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True";

        public async Task InitializeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
        }

        private InventoryDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            return new InventoryDbContext(options);
        }

        [Fact]
        public async Task ReserveNextAsync_UnderConcurrentCallsForSamePslAndDate_AssignsUniqueSequentialItemNumbers()
        {
            const int concurrentRequests = 15;
            var pslId = ProductServiceLineId.New();
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Todas las requests piden el mismo PSL + mismo día — es exactamente el escenario real
            // de dos usuarios generando código de producto para la misma línea al mismo tiempo.
            var tasks = Enumerable.Range(0, concurrentRequests).Select(async i =>
            {
                await using var context = CreateContext();
                var repository = new ProductCodeCounterRepository(context);
                return await repository.ReserveNextAsync(pslId, today, (_, itemNum) => $"codigo-{itemNum}");
            });

            var results = await Task.WhenAll(tasks);

            var itemNumbers = results.Select(r => r.ItemNumberByDay).ToList();

            // La clave primaria compuesta es (PslId, Date, ItemNumberByDay) — un duplicado acá
            // habría hecho fallar el SaveChangesAsync con violación de PK, no solo el assert.
            Assert.Equal(concurrentRequests, itemNumbers.Distinct().Count());
            Assert.Equal(Enumerable.Range(1, concurrentRequests), itemNumbers.OrderBy(n => n));

            // El PslSequenceNumber es el mismo para las 15 requests (mismo PSL, se asigna una sola
            // vez la primera vez que se ve ese PSL) — a diferencia de ItemNumberByDay, no debe variar.
            Assert.All(results, r => Assert.Equal(results[0].PslSequenceNumber, r.PslSequenceNumber));
        }
    }
}
