using MasterData.Application.ExchangeRates.Queries.GetByDate;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetExchangeRateByDateQueryHandlerTests
    {
        private readonly IExchangeRateRepository _repository = Substitute.For<IExchangeRateRepository>();

        private GetExchangeRateByDateQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNoRateRegisteredOnOrBeforeDate_ReturnsNotFound()
        {
            var date = new DateOnly(2020, 1, 1);
            _repository.GetLatestByDateAsync(date, Arg.Any<CancellationToken>()).Returns((ExchangeRate?)null);

            var result = await CreateHandler().Handle(new GetExchangeRateByDateQuery(date), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ExchangeRateErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenRateExists_ReturnsResponse()
        {
            var date = new DateOnly(2026, 8, 10);
            var rate = ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 45.5m, ExchangeRateSource.Manual);
            _repository.GetLatestByDateAsync(date, Arg.Any<CancellationToken>()).Returns(rate);

            var result = await CreateHandler().Handle(new GetExchangeRateByDateQuery(date), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(45.5m, result.Value.Rate);
            Assert.Equal("Manual", result.Value.SourceName);
        }
    }
}
