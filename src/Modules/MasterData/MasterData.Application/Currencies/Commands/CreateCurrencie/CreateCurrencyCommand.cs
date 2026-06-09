using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.CreateCurrencie
{
    public record CreateCurrencyCommand(
        string Code,
        string Name,
        string Symbol
    ):IRequest<ErrorOr<Guid>>;
}
