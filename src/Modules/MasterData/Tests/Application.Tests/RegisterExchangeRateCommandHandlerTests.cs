using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ExchangeRates.Commands.RegisterExchangeRate;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class RegisterExchangeRateCommandHandlerTests
    {
        // Registro manual de tasa (Source.Manual) — distinto del sync automático de BCV.
        private readonly IExchangeRateRepository _repository = Substitute.For<IExchangeRateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private RegisterExchangeRateCommandHandler CreateHandler() => new(_repository, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenRateIsNotPositive_ReturnsRateMustBePositive()
        {
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var command = new RegisterExchangeRateCommand(0m, ExchangeRateSource.Manual);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ExchangeRateErrors.RateMustBePositive.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsWithVenezuelaClockDate()
        {
            var venezuelaNow = new DateTime(2026, 8, 10, 8, 0, 0);
            _fiscalClock.VenezuelaNow.Returns(venezuelaNow);

            var command = new RegisterExchangeRateCommand(45.5m, ExchangeRateSource.Manual);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            // Fecha fiscal en hora Venezuela, NUNCA DateTime.UtcNow/Now del servidor.
            await _repository.Received(1).AddAsync(
                Arg.Is<ExchangeRate>(r => r.Rate == 45.5m && r.RegisterDate == venezuelaNow && r.Source == ExchangeRateSource.Manual),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
