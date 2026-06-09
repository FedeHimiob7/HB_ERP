using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.CreatePSL
{
    public record CreateProductServiceLineCommand(
    string Name,
    string Description
) : IRequest<ErrorOr<Guid>>;
}
