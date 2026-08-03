using HB_ERP.SharedKernel.Infrastructure;
using Xunit;

namespace HB_ERP.SharedKernel.Tests
{
    public sealed class FiscalClockTests
    {
        [Fact]
        public void VenezuelaNow_IsAlwaysFourHoursBehindUtc()
        {
            var clock = new FiscalClock();

            var difference = clock.UtcNow - clock.VenezuelaNow;

            Assert.True(
                Math.Abs((difference - TimeSpan.FromHours(4)).TotalSeconds) < 1,
                $"Se esperaba una diferencia de 4 horas, pero fue de {difference}.");
        }

        [Fact]
        public void VenezuelaToday_MatchesTheDatePartOfVenezuelaNow()
        {
            var clock = new FiscalClock();

            Assert.Equal(DateOnly.FromDateTime(clock.VenezuelaNow), clock.VenezuelaToday);
        }

        [Fact]
        public void VenezuelaNow_CanFallOnADifferentCalendarDayThanUtcNow()
        {
            // Ejemplo real del bug que motivó este cambio: 21:00 hora Venezuela ya es
            // 01:00 UTC del día siguiente. Cualquier comparación "por día" hecha sobre
            // UTC (como estaba antes) rompería la conciliación fiscal en ese rango horario.
            var clock = new FiscalClock();
            var utcNow = clock.UtcNow;
            var venezuelaNow = clock.VenezuelaNow;

            if (utcNow.Hour < 4)
            {
                Assert.NotEqual(DateOnly.FromDateTime(utcNow), DateOnly.FromDateTime(venezuelaNow));
            }
        }
    }
}
