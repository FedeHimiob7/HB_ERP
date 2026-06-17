using ErrorOr;
using MasterData.Application.Taxes.Models;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetById
{
    public record GetTaxByIdQuery(Guid Id) : IRequest<ErrorOr<TaxResponse>>;
}
