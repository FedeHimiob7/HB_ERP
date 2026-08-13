using MasterData.Application.Currencies.Queries.GetCurrencies;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllCurrenciesQueryHandlerTests
    {
        private readonly ICurrencyRepository _repository = Substitute.For<ICurrencyRepository>();

        private GetAllCurrenciesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllCurrenciesToResponse()
        {
            var currencies = new List<Currency> { Currency.CreateExisting(Guid.NewGuid(), "USD", "Dólar", "$", isActive: true) };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(currencies);

            var result = await CreateHandler().Handle(new GetAllCurrenciesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("USD", result.Value[0].Code);
        }
    }
}
