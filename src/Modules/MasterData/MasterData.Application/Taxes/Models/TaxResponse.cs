using MasterData.Domain.Enums;

namespace MasterData.Application.Taxes.Models
{
    public record TaxResponse(
        Guid Id,
        string Name,
        TaxType TaxType,
        string TaxTypeName,
        decimal Rate);
}
