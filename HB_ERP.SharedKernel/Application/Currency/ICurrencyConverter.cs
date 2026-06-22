namespace HB_ERP.SharedKernel.Application.Currency
{
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Convierte un monto a USD usando la tasa provista.
        /// VES: amount / exchangeRate — USD: sin cambio — EUR: amount * exchangeRate
        /// </summary>
        decimal ToUSD(decimal amount, string currencyCode, decimal exchangeRate);

        /// <summary>
        /// Convierte un monto desde USD a la moneda destino usando la tasa provista.
        /// </summary>
        decimal FromUSD(decimal amountUSD, string currencyCode, decimal exchangeRate);
    }
}
