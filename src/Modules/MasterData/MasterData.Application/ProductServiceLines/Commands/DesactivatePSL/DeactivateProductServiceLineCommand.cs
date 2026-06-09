using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.DesactivatePSL
{
    public record DeactivateProductServiceLineCommand(
     Guid Id
 ) : IRequest<ErrorOr<Success>>;
}
