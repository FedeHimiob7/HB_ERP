using ErrorOr;
using MasterData.Application.ProductServiceLines.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.UpdatePSL
{
    public record UpdateProductServiceLineCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest<ErrorOr<ProductServiceLineResponse>>;
}
