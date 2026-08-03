using HB_ERP.SharedKernel.Domain.Primitives;

namespace HB_ERP.SharedKernel.Infrastructure
{
    public sealed class FiscalClock : IFiscalClock
    {
        // Venezuela no observa horario de verano desde 2016 — el offset es siempre UTC-4.
        private static readonly TimeSpan VenezuelaOffset = TimeSpan.FromHours(-4);

        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime VenezuelaNow => DateTime.UtcNow.Add(VenezuelaOffset);
        public DateOnly VenezuelaToday => DateOnly.FromDateTime(VenezuelaNow);
    }
}
