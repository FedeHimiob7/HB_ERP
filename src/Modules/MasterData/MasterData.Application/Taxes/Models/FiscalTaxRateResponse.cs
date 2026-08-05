namespace MasterData.Application.Taxes.Models
{
    public record FiscalTaxRateResponse(
        Guid TaxId,
        decimal Rate,
        DateTime EffectiveFrom);
}
