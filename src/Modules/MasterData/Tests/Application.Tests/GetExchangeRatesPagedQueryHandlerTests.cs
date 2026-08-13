using MasterData.Application.ExchangeRates.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetExchangeRatesPagedQueryHandlerTests
    {
        private readonly IExchangeRateRepository _repository = Substitute.For<IExchangeRateRepository>();

        private GetExchangeRatesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultOrderedByRegisterDateDescending()
        {
            // Este repo, a diferencia de los demás Get*Paged de MasterData, no tiene SearchTerm —
            // ExchangeRate se pagina solo por fecha (orden descendente, ya documentado en CLAUDE.md).
            var items = new List<ExchangeRate>
            {
                ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 45.5m, ExchangeRateSource.BCV),
            };
            _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>()).Returns((items, 40));

            var result = await CreateHandler().Handle(new GetExchangeRatesPagedQuery(1, 10), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(40, result.Value.TotalCount);
        }
    }
}
