using ErrorOr;
using MasterData.Application.Taxes.Models;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetEffectiveRate
{
    public record GetEffectiveTaxRateQuery(Guid TaxId, DateOnly AsOfDate) : IRequest<ErrorOr<FiscalTaxRateResponse>>;
}
