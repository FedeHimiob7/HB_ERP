using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using MasterData.Infrastructure.Persistence;
using MasterData.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests
{
    // Primer Infrastructure.Tests del repo (ver D-7/roadmap). A diferencia de los Application.Tests,
    // acá NO se mockea el repositorio — se corre contra una instancia real de SQL Server (LocalDB),
    // porque lo que hay que probar es el patrón UPDLOCK/HOLDLOCK/Serializable en sí, algo que un
    // mock nunca podría detectar si estuviera roto (ver GenerateProductCodeCommandHandlerTests para
    // la contraparte con mocks, que sí cubre la lógica de armado del código pero no la concurrencia).
    public sealed class BranchRepositoryConcurrencyTests : IAsyncLifetime
    {
        private readonly string _connectionString =
            $"Server=(localdb)\\MSSQLLocalDB;Database=HB_ERP_Test_Branch_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True";

        public async Task InitializeAsync()
        {
            // EnsureCreated (no migraciones) alcanza acá: solo necesitamos que el esquema actual
            // exista, no reproducir el historial de migraciones fila por fila.
            await using var context = CreateContext();
            await context.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
        }

        private MasterDataDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MasterDataDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            return new MasterDataDbContext(options);
        }

        [Fact]
        public async Task ReserveNextSequenceNumberAndAddAsync_UnderConcurrentCalls_AssignsUniqueSequentialNumbers()
        {
            const int concurrentRequests = 15;

            // Cada tarea simula una request HTTP distinta: su propio DbContext (no se comparte una
            // instancia entre threads, DbContext no es thread-safe), pero todas apuntan a la misma
            // base LocalDB — es ahí donde el UPDLOCK tiene que hacer su trabajo de verdad.
            var tasks = Enumerable.Range(0, concurrentRequests).Select(async i =>
            {
                await using var context = CreateContext();
                var repository = new BranchRepository(context);
                return await repository.ReserveNextSequenceNumberAndAddAsync(
                    seq => Branch.Create(CompanyId.Singleton, $"Sucursal {i}", "Direccion Test", seq));
            });

            var results = await Task.WhenAll(tasks);

            Assert.All(results, r => Assert.False(r.IsError));
            var sequenceNumbers = results.Select(r => r.Value.SequenceNumber).ToList();

            // Si el UPDLOCK/HOLDLOCK no serializara correctamente el acceso, dos requests concurrentes
            // podrían leer el mismo MAX(SequenceNumber) y terminar asignando el mismo número — lo que
            // además violaría el índice único real de la tabla (BranchConfiguration.HasIndex(...).IsUnique()),
            // así que un fallo acá se manifestaría como duplicados o como una excepción de SaveChangesAsync.
            Assert.Equal(concurrentRequests, sequenceNumbers.Distinct().Count());
            Assert.Equal(Enumerable.Range(1, concurrentRequests), sequenceNumbers.OrderBy(n => n));
        }
    }
}
