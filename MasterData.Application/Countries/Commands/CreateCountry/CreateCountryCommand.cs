using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Commands.CreateCountry
{
    public record CreateCountryCommand(
     string Name
 ) : IRequest<ErrorOr<Guid>>;
}
