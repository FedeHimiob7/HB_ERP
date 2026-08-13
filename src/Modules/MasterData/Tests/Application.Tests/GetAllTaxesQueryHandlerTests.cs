using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Taxes.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllTaxesQueryHandlerTests
    {
        private readonly ITaxRepository _repository = Substitute.For<ITaxRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private GetAllTaxesQueryHandler CreateHandler() => new(_repository, _fiscalTaxRateRepository, _fiscalClock);

        [Fact]
        public async Task Handle_WhenTaxHasNoEffectiveRate_ReturnsZeroWithoutFailing()
        {
            var tax = Tax.Create("ISLR", TaxType.ISLR).Value;
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Tax> { tax });
            // Diccionario vacío = ningún Tax tiene tasa vigente todavía (TryGetValue falla para todos).
            _fiscalTaxRateRepository.GetEffectiveManyAsync(Arg.Any<IEnumerable<TaxId>>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<TaxId, FiscalTaxRate>());

            var result = await CreateHandler().Handle(new GetAllTaxesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(0m, result.Value[0].Rate);
        }

        [Fact]
        public async Task Handle_MapsEachTaxWithItsOwnEffectiveRate()
        {
            var taxIva = Tax.Create("IVA", TaxType.IVA).Value;
            var taxIgtf = Tax.Create("IGTF", TaxType.IGTF).Value;
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Tax> { taxIva, taxIgtf });

            var rateIva = FiscalTaxRate.Create(taxIva.Id, 0.16m, DateTime.UtcNow).Value;
            // A taxIgtf deliberadamente no le ponemos entrada en el diccionario, para confirmar
            // que cada Tax resuelve su propia tasa de forma independiente (no "arrastra" la del anterior).
            _fiscalTaxRateRepository.GetEffectiveManyAsync(Arg.Any<IEnumerable<TaxId>>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<TaxId, FiscalTaxRate> { [taxIva.Id] = rateIva });

            var result = await CreateHandler().Handle(new GetAllTaxesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(0.16m, result.Value.Single(t => t.Id == taxIva.Id.Value).Rate);
            Assert.Equal(0m, result.Value.Single(t => t.Id == taxIgtf.Id.Value).Rate);
        }
    }
}
