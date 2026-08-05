using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class FiscalTaxRateTests
    {
        private static readonly TaxId SampleTaxId = TaxId.New();
        private static readonly DateTime SampleEffectiveFrom = new(2026, 7, 30, 0, 0, 0);

        [Fact]
        public void Create_WithNonNegativeRate_Succeeds()
        {
            var result = FiscalTaxRate.Create(SampleTaxId, 0.16m, SampleEffectiveFrom);

            Assert.False(result.IsError);
            Assert.Equal(SampleTaxId, result.Value.TaxId);
            Assert.Equal(0.16m, result.Value.Rate);
            Assert.Equal(SampleEffectiveFrom, result.Value.EffectiveFrom);
        }

        [Fact]
        public void Create_WithZeroRate_Succeeds()
        {
            // El tramo en bolívares del IGTF es 0% — la alícuota debe permitir cero.
            var result = FiscalTaxRate.Create(SampleTaxId, 0m, SampleEffectiveFrom);

            Assert.False(result.IsError);
            Assert.Equal(0m, result.Value.Rate);
        }

        [Fact]
        public void Create_WithNegativeRate_Fails()
        {
            var result = FiscalTaxRate.Create(SampleTaxId, -0.01m, SampleEffectiveFrom);

            Assert.True(result.IsError);
            Assert.Equal("FiscalTaxRate.RateMustBeNonNegative", result.FirstError.Code);
        }

        [Fact]
        public void Create_PersistsTheEffectiveFromDatePassedByTheCaller()
        {
            var venezuelaNow = new DateTime(2026, 1, 1, 23, 0, 0);

            var result = FiscalTaxRate.Create(SampleTaxId, 0.16m, venezuelaNow);

            Assert.Equal(venezuelaNow, result.Value.EffectiveFrom);
        }
    }
}
