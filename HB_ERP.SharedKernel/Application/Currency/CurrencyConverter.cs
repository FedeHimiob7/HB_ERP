namespace HB_ERP.SharedKernel.Application.Currency
{
    internal sealed class CurrencyConverter : ICurrencyConverter
    {
        public decimal ToUSD(decimal amount, string currencyCode, decimal exchangeRate)
        {
            return currencyCode.ToUpper() switch
            {
                "USD" => amount,
                "VES" => exchangeRate == 0 ? amount : amount / exchangeRate,
                "EUR" => amount * exchangeRate,
                _ => amount
            };
        }

        public decimal FromUSD(decimal amountUSD, string currencyCode, decimal exchangeRate)
        {
            return currencyCode.ToUpper() switch
            {
                "USD" => amountUSD,
                "VES" => amountUSD * exchangeRate,
                "EUR" => exchangeRate == 0 ? amountUSD : amountUSD / exchangeRate,
                _ => amountUSD
            };
        }
    }
}
