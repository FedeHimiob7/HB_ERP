using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ExchangeRates.Commands.SyncFromBCV;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class SyncExchangeRateFromBCVCommandHandlerTests
    {
        private readonly IBCVRateScrapingService _scrapingService = Substitute.For<IBCVRateScrapingService>();
        private readonly IExchangeRateRepository _repository = Substitute.For<IExchangeRateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private SyncExchangeRateFromBCVCommandHandler CreateHandler() => new(_scrapingService, _repository, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenBCVReturnsZeroOrNegative_ReturnsNoRateAvailable()
        {
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(0m);

            var result = await CreateHandler().Handle(new SyncExchangeRateFromBCVCommand(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ExchangeRateErrors.NoRateAvailable.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFetchedRateEqualsLatest_DoesNotDuplicateAndReturnsWasCreatedFalse()
        {
            // Regla de negocio clave: si la tasa de BCV no cambió respecto a la última guardada,
            // NO se crea un registro nuevo (evita ruido en la tabla de historial).
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(45.5m);
            var latest = ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 45.5m, ExchangeRateSource.BCV);
            _repository.GetLatestAsync(Arg.Any<CancellationToken>()).Returns(latest);

            var result = await CreateHandler().Handle(new SyncExchangeRateFromBCVCommand(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(result.Value.WasCreated);
            Assert.Equal(latest.Id.Value, result.Value.Id);

            await _repository.DidNotReceive().AddAsync(Arg.Any<ExchangeRate>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenFetchedRateDiffersFromLatest_CreatesNewRateAndReturnsWasCreatedTrue()
        {
            _scrapingService.GetRateAsync(Arg.Any<CancellationToken>()).Returns(46.0m);
            var latest = ExchangeRate.CreateExisting(Guid.NewGuid(), DateTime.UtcNow, 45.5m, ExchangeRateSource.BCV);
            _repository.GetLatestAsync(Arg.Any<CancellationToken>()).Returns(latest);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new SyncExchangeRateFromBCVCommand(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.True(result.Value.WasCreated);
            Assert.Equal(46.0m, result.Value.Rate);

            await _repository.Received(1).AddAsync(
                Arg.Is<ExchangeRate>(r => r.Rate == 46.0m && r.Source == ExchangeRateSource.BCV),
                Arg.Any<CancellationToken>());
        }
    }
}
