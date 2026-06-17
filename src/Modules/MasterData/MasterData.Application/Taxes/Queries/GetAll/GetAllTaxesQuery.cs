using ErrorOr;
using MasterData.Application.Taxes.Models;
using MediatR;

namespace MasterData.Application.Taxes.Queries.GetAll
{
    public record GetAllTaxesQuery() : IRequest<ErrorOr<IReadOnlyList<TaxResponse>>>;
}
