using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.DeactivateCurrency
{
    public record DeactivateCurrencyCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
