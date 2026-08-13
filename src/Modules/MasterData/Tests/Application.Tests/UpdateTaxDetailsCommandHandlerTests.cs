using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Application.Taxes.Commands.UpdateTaxDetails;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateTaxDetailsCommandHandlerTests
    {
        // D-4: UpdateTaxDetails solo toca la identidad (Name/TaxType) del Tax, NUNCA la alícuota —
        // por eso no depende de IFiscalTaxRateRepository para escribir, solo para leer la vigente
        // y devolverla en la respuesta.
        private readonly ITaxRepository _repository = Substitute.For<ITaxRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly Guid _taxGuid = Guid.NewGuid();

        private UpdateTaxDetailsCommandHandler CreateHandler() => new(_repository, _fiscalTaxRateRepository, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenTaxDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns((Tax?)null);

            var command = new UpdateTaxDetailsCommand(_taxGuid, "IVA", TaxType.IVA);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(TaxErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesIdentityAndReturnsCurrentEffectiveRate()
        {
            var tax = Tax.Create("IVA Viejo", TaxType.IVA).Value;
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns(tax);

            var today = new DateOnly(2026, 8, 10);
            _fiscalClock.VenezuelaToday.Returns(today);

            // La tasa vigente NO se toca en este handler, solo se lee para armar la respuesta.
            var currentRate = FiscalTaxRate.Create(tax.Id, 0.16m, new DateTime(2026, 6, 1)).Value;
            _fiscalTaxRateRepository.GetEffectiveAsync(tax.Id, today, Arg.Any<CancellationToken>()).Returns(currentRate);

            var command = new UpdateTaxDetailsCommand(_taxGuid, "IVA Nuevo", TaxType.IGTF);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("IVA Nuevo", result.Value.Name);
            Assert.Equal(TaxType.IGTF, result.Value.TaxType);
            Assert.Equal(0.16m, result.Value.Rate);

            await _fiscalTaxRateRepository.DidNotReceive().AddAsync(Arg.Any<FiscalTaxRate>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenNoEffectiveRateExists_ReturnsZeroRateWithoutFailing()
        {
            // Caso borde: un Tax recién migrado sin ninguna FiscalTaxRate vigente todavía —
            // el handler no debe fallar, solo reportar 0m (mismo fallback que GetAll/GetById).
            var tax = Tax.Create("IVA", TaxType.IVA).Value;
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns(tax);
            _fiscalTaxRateRepository.GetEffectiveAsync(Arg.Any<TaxId>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
                .Returns((FiscalTaxRate?)null);

            var command = new UpdateTaxDetailsCommand(_taxGuid, "IVA", TaxType.IVA);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(0m, result.Value.Rate);
        }
    }
}
