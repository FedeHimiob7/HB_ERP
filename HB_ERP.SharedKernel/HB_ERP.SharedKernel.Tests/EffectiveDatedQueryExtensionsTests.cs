using HB_ERP.SharedKernel.Domain.Primitives;
using HB_ERP.SharedKernel.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HB_ERP.SharedKernel.Tests
{
    // GetEffectiveAsOfAsync es lógica LINQ-to-Entities pura (Where + OrderByDescending +
    // FirstOrDefaultAsync) — se prueba con el proveedor InMemory de EF Core en vez de mocks,
    // porque lo que hay que verificar es que la traducción de la query hace lo correcto,
    // no solo que la firma del método se llame.
    public sealed class EffectiveDatedQueryExtensionsTests
    {
        private sealed class TestVersionedValue : IEffectiveDated
        {
            public int Id { get; set; }
            public decimal Rate { get; set; }
            public DateTime EffectiveFrom { get; set; }
        }

        private sealed class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
            public DbSet<TestVersionedValue> Values => Set<TestVersionedValue>();
        }

        private static TestDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TestDbContext(options);
        }

        [Fact]
        public async Task GetEffectiveAsOfAsync_WhenMultipleVersionsExist_ReturnsTheMostRecentOnOrBeforeTheDate()
        {
            await using var context = CreateInMemoryContext();
            context.Values.AddRange(
                new TestVersionedValue { Id = 1, Rate = 0.12m, EffectiveFrom = new DateTime(2026, 1, 1) },
                new TestVersionedValue { Id = 2, Rate = 0.16m, EffectiveFrom = new DateTime(2026, 6, 1) },
                new TestVersionedValue { Id = 3, Rate = 0.20m, EffectiveFrom = new DateTime(2026, 9, 1) });
            await context.SaveChangesAsync();

            var result = await context.Values.GetEffectiveAsOfAsync(new DateOnly(2026, 8, 10));

            // A esa fecha, la versión de septiembre todavía no rige — la vigente es la de junio.
            Assert.NotNull(result);
            Assert.Equal(2, result!.Id);
            Assert.Equal(0.16m, result.Rate);
        }

        [Fact]
        public async Task GetEffectiveAsOfAsync_IgnoresVersionsThatStartAfterTheDate()
        {
            await using var context = CreateInMemoryContext();
            context.Values.Add(new TestVersionedValue { Id = 1, Rate = 0.20m, EffectiveFrom = new DateTime(2027, 1, 1) });
            await context.SaveChangesAsync();

            var result = await context.Values.GetEffectiveAsOfAsync(new DateOnly(2026, 8, 10));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEffectiveAsOfAsync_WhenNoVersionExists_ReturnsNull()
        {
            await using var context = CreateInMemoryContext();

            var result = await context.Values.GetEffectiveAsOfAsync(new DateOnly(2026, 8, 10));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEffectiveAsOfAsync_WhenAsOfDateEqualsEffectiveFromDate_IncludesThatVersion()
        {
            // El límite superior es asOfDate + 1 día (EffectiveFrom < upperBound), así que un
            // registro cuyo EffectiveFrom cae el mismo día consultado SÍ debe contar como vigente.
            await using var context = CreateInMemoryContext();
            context.Values.Add(new TestVersionedValue { Id = 1, Rate = 0.16m, EffectiveFrom = new DateTime(2026, 8, 10, 14, 0, 0) });
            await context.SaveChangesAsync();

            var result = await context.Values.GetEffectiveAsOfAsync(new DateOnly(2026, 8, 10));

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }
    }
}
