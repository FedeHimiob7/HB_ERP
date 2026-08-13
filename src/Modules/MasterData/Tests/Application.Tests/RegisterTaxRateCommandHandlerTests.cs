using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Application.Taxes.Commands.RegisterTaxRate;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class RegisterTaxRateCommandHandlerTests
    {
        // D-4: cada cambio de alícuota crea una FiscalTaxRate nueva, NUNCA edita una existente —
        // este handler es el único punto de entrada para eso.
        private readonly ITaxRepository _taxRepository = Substitute.For<ITaxRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly Guid _taxGuid = Guid.NewGuid();

        private RegisterTaxRateCommandHandler CreateHandler() => new(_taxRepository, _fiscalTaxRateRepository, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenTaxDoesNotExist_ReturnsNotFound()
        {
            _taxRepository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns((Tax?)null);

            var result = await CreateHandler().Handle(new RegisterTaxRateCommand(_taxGuid, 0.16m), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(TaxErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenRateIsNegative_ReturnsRateMustBeNonNegative()
        {
            var tax = Tax.Create("IVA", TaxType.IVA).Value;
            _taxRepository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns(tax);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new RegisterTaxRateCommand(_taxGuid, -1m), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTaxRateErrors.RateMustBeNonNegative.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_RegistersNewRateWithVenezuelaClockDate()
        {
            var tax = Tax.Create("IVA", TaxType.IVA).Value;
            _taxRepository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns(tax);

            var venezuelaNow = new DateTime(2026, 8, 10, 9, 30, 0);
            _fiscalClock.VenezuelaNow.Returns(venezuelaNow);

            var result = await CreateHandler().Handle(new RegisterTaxRateCommand(_taxGuid, 0.22m), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            // El TaxId real que debe usar la nueva FiscalTaxRate es el del comando (request.TaxId),
            // no el Id interno de la instancia `tax` (que se generó con Tax.Create y es aleatorio) —
            // el handler arma FiscalTaxRate.Create con TaxId.Create(request.TaxId).
            await _fiscalTaxRateRepository.Received(1).AddAsync(
                Arg.Is<FiscalTaxRate>(r => r.Rate == 0.22m && r.EffectiveFrom == venezuelaNow && r.TaxId == TaxId.Create(_taxGuid)),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
