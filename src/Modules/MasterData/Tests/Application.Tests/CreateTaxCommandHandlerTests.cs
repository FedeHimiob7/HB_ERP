using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Application.Taxes.Commands.CreateTax;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateTaxCommandHandlerTests
    {
        // D-4: CreateTax ahora crea Tax + FiscalTaxRate de forma atómica (antes Tax tenía su propio
        // Rate). Estos tests son los que verifican ese comportamiento nuevo.
        private readonly ITaxRepository _taxRepository = Substitute.For<ITaxRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private CreateTaxCommandHandler CreateHandler() => new(_taxRepository, _fiscalTaxRateRepository, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenRateIsNegative_ReturnsRateMustBeNonNegative()
        {
            // El nombre/tipo son válidos (Tax.Create pasa), pero la tasa es negativa —
            // el guard vive en FiscalTaxRate.Create, no en Tax.Create.
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var command = new CreateTaxCommand("IVA", TaxType.IVA, -0.01m);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTaxRateErrors.RateMustBeNonNegative.Code, result.FirstError.Code);
            // Como falló la segunda validación, no debería haber persistido nada.
            await _taxRepository.DidNotReceive().AddAsync(Arg.Any<Tax>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsTaxAndFirstFiscalTaxRateAtomically()
        {
            var venezuelaNow = new DateTime(2026, 8, 10, 12, 0, 0);
            _fiscalClock.VenezuelaNow.Returns(venezuelaNow);

            var command = new CreateTaxCommand("IVA", TaxType.IVA, 0.16m);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _taxRepository.Received(1).AddAsync(Arg.Any<Tax>(), Arg.Any<CancellationToken>());
            // La primera FiscalTaxRate se crea con la fecha del IFiscalClock, no DateTime.Now del servidor.
            await _fiscalTaxRateRepository.Received(1).AddAsync(
                Arg.Is<FiscalTaxRate>(r => r.Rate == 0.16m && r.EffectiveFrom == venezuelaNow),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
