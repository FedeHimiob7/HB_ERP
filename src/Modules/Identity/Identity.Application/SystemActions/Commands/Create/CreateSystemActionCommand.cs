using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Identity.Application.SystemActions.Commands.Create
{
    public sealed record CreateSystemActionCommand(
     string Name,
     string Description) : IRequest<ErrorOr<Guid>>;
}
