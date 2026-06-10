using ErrorOr;
using MasterData.Application.ProductServiceLines.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Queries.GetPaged
{
    public record GetProductServiceLinesPagedQuery(
    int PageNumber,
    int PageSize,
    string? SearchTerm = null
) : IRequest<ErrorOr<PagedProductServiceLinesResult>>;
}
