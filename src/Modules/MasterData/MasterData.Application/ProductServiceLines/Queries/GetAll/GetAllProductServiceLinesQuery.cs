using ErrorOr;
using MasterData.Application.ProductServiceLines.Models;
using MediatR;

namespace MasterData.Application.ProductServiceLines.Queries.GetAll
{
    public record GetAllProductServiceLinesQuery() : IRequest<ErrorOr<IReadOnlyList<ProductServiceLineResponse>>>;
}
