using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ExchangeRates.Queries.GetCurrent;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCurrentExchangeRateQueryHandlerTests
    {
        private readonly IExchangeRateRepository _repository = Substitute.For<IExchangeRateRepository>();
        private readonly IBCVRateScrapingService _scrapingService = Substitute.For<IBCVRateScrapingService>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private GetCurrentExchangeRateQueryHandler CreateHandler() => new(_repository, _scrapingService, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenBCVUnavailableAndNoStoredRate_ReturnsNoRateAvailable()
        {
            // BCV no responde (rate <= 0) Y nunca hubo ningún sync previo — ni fresco ni fallback.
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(0m);
            _repository.GetLatestAsync(Arg.Any<CancellationToken>()).Returns((ExchangeRate?)null);

            var result = await CreateHandler().Handle(new GetCurrentExchangeRateQuery(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ExchangeRateErrors.NoRateAvailable.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenBCVUnavailableButThereIsAStoredRate_ReturnsFallback()
        {
            // BCV no responde, pero sí hay una tasa guardada de una sync anterior — el fallback documentado.
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(0m);
            var fallback = ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 44.0m, ExchangeRateSource.BCV);
            _repository.GetLatestAsync(Arg.Any<CancellationToken>()).Returns(fallback);

            var result = await CreateHandler().Handle(new GetCurrentExchangeRateQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(44.0m, result.Value.Rate);
            await _repository.DidNotReceive().AddAsync(Arg.Any<ExchangeRate>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenFreshRateDiffersFromLatest_SavesAndReturnsFreshRate()
        {
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(46.0m);
            var latest = ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 45.5m, ExchangeRateSource.BCV);
            _repository.GetLatestAsync(Arg.Any<CancellationToken>()).Returns(latest);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new GetCurrentExchangeRateQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(46.0m, result.Value.Rate);
            await _repository.Received(1).AddAsync(Arg.Any<ExchangeRate>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenFreshRateEqualsLatest_DoesNotSaveAndReturnsLatest()
        {
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(45.5m);
            var latest = ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 45.5m, ExchangeRateSource.BCV);
            _repository.GetLatestAsync(Arg.Any<CancellationToken>()).Returns(latest);

            var result = await CreateHandler().Handle(new GetCurrentExchangeRateQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(45.5m, result.Value.Rate);
            await _repository.DidNotReceive().AddAsync(Arg.Any<ExchangeRate>(), Arg.Any<CancellationToken>());
        }
    }
}
