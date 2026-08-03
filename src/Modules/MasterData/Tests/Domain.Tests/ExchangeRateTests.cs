using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using Xunit;

namespace Domain.Tests
{
    public sealed class ExchangeRateTests
    {
        private static readonly DateTime SampleRegisterDate = new(2026, 7, 30, 14, 30, 0);

        [Fact]
        public void Create_WithPositiveRate_Succeeds()
        {
            var result = ExchangeRate.Create(150.25m, ExchangeRateSource.BCV, SampleRegisterDate);

            Assert.False(result.IsError);
            Assert.Equal(150.25m, result.Value.Rate);
            Assert.Equal(ExchangeRateSource.BCV, result.Value.Source);
            Assert.Equal(SampleRegisterDate, result.Value.RegisterDate);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithNonPositiveRate_Fails(decimal rate)
        {
            var result = ExchangeRate.Create(rate, ExchangeRateSource.Manual, SampleRegisterDate);

            Assert.True(result.IsError);
            Assert.Equal("ExchangeRate.RateMustBePositive", result.FirstError.Code);
        }

        [Fact]
        public void Create_PersistsTheRegisterDatePassedByTheCaller()
        {
            // La entidad no debe fijar su propia fecha (p. ej. DateTime.UtcNow) — la fecha fiscal
            // la decide el llamador (IFiscalClock.VenezuelaNow en producción), para que RegisterDate
            // siempre represente el calendario de Venezuela y no la hora del servidor.
            var venezuelaNow = new DateTime(2026, 1, 1, 23, 0, 0);

            var result = ExchangeRate.Create(100m, ExchangeRateSource.BCV, venezuelaNow);

            Assert.Equal(venezuelaNow, result.Value.RegisterDate);
        }
    }
}
