using MasterData.Application.Currencies.Queries.GetCurrencyById;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCurrencyByIdQueryHandlerTests
    {
        private readonly ICurrencyRepository _repository = Substitute.For<ICurrencyRepository>();
        private readonly Guid _currencyGuid = Guid.NewGuid();

        private GetCurrencyByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CurrencyId>(), Arg.Any<CancellationToken>()).Returns((Currency?)null);

            var result = await CreateHandler().Handle(new GetCurrencyByIdQuery(_currencyGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("Currency.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var currency = Currency.CreateExisting(_currencyGuid, "USD", "Dólar", "$", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CurrencyId>(), Arg.Any<CancellationToken>()).Returns(currency);

            var result = await CreateHandler().Handle(new GetCurrencyByIdQuery(_currencyGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_currencyGuid, result.Value.Id);
        }
    }
}
