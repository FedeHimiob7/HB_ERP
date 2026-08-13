using MasterData.Application.Taxes.Queries.GetEffectiveRate;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetEffectiveTaxRateQueryHandlerTests
    {
        // Este query es el consumidor principal de GetEffectiveAsOfAsync (vía FiscalTaxRateRepository) —
        // "resuelve la tasa vigente a la fecha del documento", nunca "el valor actual".
        private readonly IFiscalTaxRateRepository _repository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly Guid _taxGuid = Guid.NewGuid();

        private GetEffectiveTaxRateQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNoRateEffectiveAtThatDate_ReturnsNotFound()
        {
            var asOfDate = new DateOnly(2020, 1, 1);
            _repository.GetEffectiveAsync(Arg.Any<TaxId>(), asOfDate, Arg.Any<CancellationToken>())
                .Returns((FiscalTaxRate?)null);

            var result = await CreateHandler().Handle(new GetEffectiveTaxRateQuery(_taxGuid, asOfDate), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTaxRateErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenRateExists_ReturnsResponseWithRateAndEffectiveFrom()
        {
            var asOfDate = new DateOnly(2026, 8, 10);
            var effectiveFrom = new DateTime(2026, 6, 1);
            var rate = FiscalTaxRate.Create(TaxId.Create(_taxGuid), 0.16m, effectiveFrom).Value;
            _repository.GetEffectiveAsync(Arg.Any<TaxId>(), asOfDate, Arg.Any<CancellationToken>()).Returns(rate);

            var result = await CreateHandler().Handle(new GetEffectiveTaxRateQuery(_taxGuid, asOfDate), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_taxGuid, result.Value.TaxId);
            Assert.Equal(0.16m, result.Value.Rate);
            Assert.Equal(effectiveFrom, result.Value.EffectiveFrom);
        }
    }
}
