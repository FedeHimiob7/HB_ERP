using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class FiscalTaxRateErrors
    {
        public static readonly Error RateMustBeNonNegative =
            Error.Validation("FiscalTaxRate.RateMustBeNonNegative", "La alícuota debe ser mayor o igual a cero.");

        public static readonly Error NotFound =
            Error.NotFound("FiscalTaxRate.NotFound", "No se encontró una alícuota vigente para el impuesto solicitado.");
    }
}
