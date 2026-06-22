using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class ExchangeRateErrors
    {
        public static readonly Error RateMustBePositive =
            Error.Validation("ExchangeRate.RateMustBePositive", "La tasa de cambio debe ser mayor a cero.");

        public static readonly Error NotFound =
            Error.NotFound("ExchangeRate.NotFound", "No se encontró la tasa de cambio.");

        public static readonly Error NoRateAvailable =
            Error.NotFound("ExchangeRate.NoRateAvailable", "No hay ninguna tasa de cambio registrada en el sistema.");
    }
}
