using MasterData.Application.Currencies.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCurrenciesPagedQueryHandlerTests
    {
        private readonly ICurrencyRepository _repository = Substitute.For<ICurrencyRepository>();

        private GetCurrenciesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultUsingFilterPassedByCaller()
        {
            var currencies = new List<Currency> { Currency.CreateExisting(Guid.NewGuid(), "USD", "Dólar", "$", isActive: true) };
            var filter = new CurrencyFilter(1, 10, "USD");
            _repository.GetPagedAsync(filter, Arg.Any<CancellationToken>()).Returns((currencies, 6));

            var result = await CreateHandler().Handle(new GetCurrenciesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(6, result.Value.TotalCount);
        }
    }
}
